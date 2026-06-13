using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CastleCarryableBox : MonoBehaviour
{
    [SerializeField] private Vector2 carriedVelocity = Vector2.zero;

    private Rigidbody2D body;
    private Collider2D boxCollider;
    private Transform holder;
    private Vector3 localHoldOffset;
    private bool wasKinematic;
    private float originalGravityScale;

    public bool IsHeld => holder != null;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        wasKinematic = body.bodyType == RigidbodyType2D.Kinematic;
        originalGravityScale = body.gravityScale;
    }

    private void LateUpdate()
    {
        if (holder == null)
        {
            return;
        }

        transform.position = holder.TransformPoint(localHoldOffset);
    }

    public void PickUp(Transform newHolder, Vector3 holdOffset)
    {
        if (newHolder == null || IsHeld)
        {
            return;
        }

        holder = newHolder;
        localHoldOffset = holdOffset;
        body.linearVelocity = carriedVelocity;
        body.angularVelocity = 0f;
        body.gravityScale = 0f;
        body.bodyType = RigidbodyType2D.Kinematic;
        boxCollider.enabled = false;
        transform.position = holder.TransformPoint(localHoldOffset);
    }

    public void UpdateHoldOffset(Vector3 holdOffset)
    {
        localHoldOffset = holdOffset;
    }

    public void Place(Vector3 worldPosition)
    {
        if (!IsHeld)
        {
            return;
        }

        holder = null;
        transform.position = worldPosition;
        boxCollider.enabled = true;
        body.bodyType = wasKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        body.gravityScale = originalGravityScale;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
    }
}
