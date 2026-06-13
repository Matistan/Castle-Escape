using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CastleEnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 startPosition;
    private int direction = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float offsetFromStart = transform.position.x - startPosition.x;
        if (Mathf.Abs(offsetFromStart) >= patrolDistance)
        {
            direction = offsetFromStart > 0f ? -1 : 1;
        }

        body.linearVelocity = new Vector2(direction * moveSpeed, body.linearVelocity.y);
        spriteRenderer.flipX = direction > 0;
        PlayAnimation(moveSpeed > 0.05f ? "Run" : "Idle");
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
