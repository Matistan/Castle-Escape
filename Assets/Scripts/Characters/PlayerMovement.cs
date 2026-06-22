using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerId playerId;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float carryMoveSpeedMultiplier = 0.75f;
    [SerializeField] private float carryJumpForceMultiplier = 0.85f;
    [SerializeField] private float pickupRadius = 0.9f;
    [SerializeField] private Vector2 carryOffset = new Vector2(0.55f, 0.55f);
    [SerializeField] private Vector2 placeOffset = new Vector2(0.9f, -0.35f);

    [Header("Air Physics")]
    [SerializeField] private float airAcceleration = 30f;
    [SerializeField] private float airDrag = 15f;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.85f;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private PlayerInputSnapshot input;
    private Box heldBox;
    private float jumpBufferTimer;
    private int facingDirection = 1;

    public PlayerId PlayerId
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
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        InputManager inputManager = InputManager.Instance;
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

        bool grounded = IsGrounded();

        if (grounded)
        {
            velocity.x = input.Move.x * effectiveMoveSpeed;
        }
        else
        {
            float targetAirSpeed = input.Move.x * effectiveMoveSpeed;

            if (input.Move.x != 0)
            {
                // Smoothly accelerate towards the direction we are pressing
                velocity.x = Mathf.MoveTowards(velocity.x, targetAirSpeed, airAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                // Smoothly slow down horizontal speed if no keys are pressed
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, airDrag * Time.fixedDeltaTime);
            }
        }

        if (jumpBufferTimer > 0f && grounded)
        {
            velocity.y = effectiveJumpForce;
            jumpBufferTimer = 0f;
            grounded = false;
            SettingsManager.PlaySfx(jumpClip, transform.position, jumpVolume);
        }

        body.linearVelocity = velocity;
        UpdateFacing(input.Move.x);
        UpdateAnimation(input.Move.x, grounded);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (hit.collider == null) return false;

        float playerFeetY = boxCollider2D.bounds.min.y;
        float platformSurfaceY = hit.point.y;
        bool isStandingOnGround = playerFeetY - platformSurfaceY >= 0.01f;

        bool isStationary = rigidBody2D.linearVelocityY <= 0.01f;

        return isStationary && isStandingOnGround;
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
        if (!Lever.TryFindNearestInRange(transform.position, pickupRadius, out Lever nearestLever))
        {
            return false;
        }

        nearestLever.Toggle();
        return true;
    }

    private Box FindNearestCarryableBox()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        Box nearestBox = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Box box = hits[i].GetComponentInParent<Box>();
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
