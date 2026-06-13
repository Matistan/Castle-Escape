using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float alertMoveSpeed = 3.5f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float detectionRadius = 4f;
    [SerializeField] private float obstacleCheckDistance = 0.55f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 startPosition;
    private int direction = 1;
    private bool isResetting;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        PlayerMovement closestPlayer = FindClosestPlayer(out float closestDistance);
        bool isAlert = closestPlayer != null && closestDistance <= detectionRadius;
        float currentSpeed = isAlert ? alertMoveSpeed : moveSpeed;

        if (isAlert)
        {
            direction = closestPlayer.transform.position.x >= transform.position.x ? 1 : -1;
        }
        else
        {
            UpdatePatrolDirection();
        }

        if (IsBlockedByBox(direction))
        {
            direction *= -1;
        }

        body.linearVelocity = new Vector2(direction * currentSpeed, body.linearVelocity.y);
        spriteRenderer.flipX = direction > 0;
        PlayAnimation(currentSpeed > 0.05f ? "Run" : "Idle");
    }

    private void UpdatePatrolDirection()
    {
        float offsetFromStart = transform.position.x - startPosition.x;
        if (Mathf.Abs(offsetFromStart) >= patrolDistance)
        {
            direction = offsetFromStart > 0f ? -1 : 1;
        }
    }

    private PlayerMovement FindClosestPlayer(out float closestDistance)
    {
        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        PlayerMovement closestPlayer = null;
        closestDistance = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, players[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = players[i];
            }
        }

        return closestPlayer;
    }

    private bool IsBlockedByBox(int moveDirection)
    {
        Vector2 origin = body.position;
        Vector2 castDirection = Vector2.right * moveDirection;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, castDirection, obstacleCheckDistance);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null)
            {
                continue;
            }

            Box box = hits[i].collider.GetComponentInParent<Box>();
            if (box != null && !box.IsHeld)
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryResetLevel(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryResetLevel(other);
    }

    private void TryResetLevel(Collider2D other)
    {
        if (isResetting || other.GetComponentInParent<PlayerMovement>() == null)
        {
            return;
        }

        isResetting = true;
        LevelReset.RequestReset();
    }

    private void PlayAnimation(string stateName)
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            return;
        }

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (!currentState.IsName(stateName))
        {
            animator.Play(stateName);
        }
    }
}
