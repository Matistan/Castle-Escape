using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CastlePlayerMovement : MonoBehaviour
{
    [SerializeField] private CastlePlayerId playerId;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float carryMoveSpeedMultiplier = 0.75f;
    [SerializeField] private float carryJumpForceMultiplier = 0.85f;
    [SerializeField] private float pickupRadius = 0.9f;
    [SerializeField] private Vector2 carryOffset = new Vector2(0.55f, 0.55f);
    [SerializeField] private Vector2 placeOffset = new Vector2(0.9f, -0.35f);
    [SerializeField] private LayerMask groundLayers = ~0;

    private Rigidbody2D body;
    private Collider2D bodyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private CastlePlayerInputSnapshot input;
    private CastleCarryableBox heldBox;
    private float jumpBufferTimer;
    private int facingDirection = 1;

    public CastlePlayerId PlayerId
    {
        get => playerId;
        set => playerId = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CastleInputManager inputManager = CastleInputManager.Instance;
        if (inputManager == null)
        {
            input = default;
            jumpBufferTimer = 0f;
            PlayAnimation("Idle");
            return;
        }

        input = inputManager.GetSnapshot(playerId);
        if (input.JumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (input.InteractPressed)
        {
            if (!TryInteractWithLever())
            {
                ToggleBoxCarry();
            }
        }

        UpdateHeldBoxOffset();
    }

    private void FixedUpdate()
    {
        float effectiveMoveSpeed = heldBox != null ? moveSpeed * carryMoveSpeedMultiplier : moveSpeed;
        float effectiveJumpForce = heldBox != null ? jumpForce * carryJumpForceMultiplier : jumpForce;

        Vector2 velocity = body.linearVelocity;
        velocity.x = input.Move.x * effectiveMoveSpeed;

        bool grounded = IsGrounded();
        if (jumpBufferTimer > 0f && grounded)
        {
            velocity.y = effectiveJumpForce;
            jumpBufferTimer = 0f;
            grounded = false;
        }

        body.linearVelocity = velocity;
        UpdateFacing(input.Move.x);
        UpdateAnimation(input.Move.x, grounded);
    }

    private bool IsGrounded()
    {
        return bodyCollider.IsTouchingLayers(groundLayers);
    }

    private void UpdateFacing(float moveX)
    {
        if (moveX > 0.05f)
        {
            spriteRenderer.flipX = false;
            facingDirection = 1;
        }
        else if (moveX < -0.05f)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1;
        }
    }

    private void ToggleBoxCarry()
    {
        if (heldBox != null)
        {
            Vector3 dropPosition = transform.position + new Vector3(placeOffset.x * facingDirection, placeOffset.y, 0f);
            heldBox.Place(dropPosition);
            heldBox = null;
            return;
        }

        heldBox = FindNearestCarryableBox();
        if (heldBox != null)
        {
            heldBox.PickUp(transform, GetHoldOffset());
        }
    }

    private bool TryInteractWithLever()
    {
        if (!CastleLever.TryFindNearestInRange(transform.position, pickupRadius, out CastleLever nearestLever))
        {
            return false;
        }

        nearestLever.Toggle();
        return true;
    }

    private CastleCarryableBox FindNearestCarryableBox()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        CastleCarryableBox nearestBox = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            CastleCarryableBox box = hits[i].GetComponentInParent<CastleCarryableBox>();
            if (box == null || box.IsHeld)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, box.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBox = box;
            }
        }

        return nearestBox;
    }

    private void UpdateHeldBoxOffset()
    {
        if (heldBox != null)
        {
            heldBox.UpdateHoldOffset(GetHoldOffset());
        }
    }

    private Vector3 GetHoldOffset()
    {
        return new Vector3(carryOffset.x * facingDirection, carryOffset.y, 0f);
    }

    private void UpdateAnimation(float moveX, bool grounded)
    {
        if (!grounded)
        {
            PlayAnimation("Jump");
        }
        else if (Mathf.Abs(moveX) > 0.05f)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Idle");
        }
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
