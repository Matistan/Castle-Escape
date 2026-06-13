using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Box : MonoBehaviour
{
    [SerializeField] private Vector2 carriedVelocity = Vector2.zero;

    [SerializeField] private float stackSearchHalfWidth = 0.45f;
    [SerializeField] private float heldScaleMultiplier = 0.75f;

    private Rigidbody2D body;
    private Vector3 originalScale;
    private Collider2D boxCollider;
    private Transform holder;
    private Vector3 localHoldOffset;
    private bool wasKinematic;
    private float originalGravityScale;
    private float stackHeight;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float dropVolume = 0.9f;

    public bool IsHeld => holder != null;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        wasKinematic = body.bodyType == RigidbodyType2D.Kinematic;
        originalGravityScale = body.gravityScale;
        stackHeight = boxCollider.bounds.size.y;
        originalScale = transform.localScale;
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
        transform.localScale = originalScale * heldScaleMultiplier;
        transform.position = holder.TransformPoint(localHoldOffset);

        SettingsManager.PlaySfx(pickupClip, transform.position, pickupVolume);
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
        transform.localScale = originalScale;
        transform.position = ResolveStackedPosition(worldPosition);
        boxCollider.enabled = true;
        body.bodyType = wasKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        body.gravityScale = originalGravityScale;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;

        SettingsManager.PlaySfx(dropClip, transform.position, dropVolume);
    }

    private Vector3 ResolveStackedPosition(Vector3 worldPosition)
    {
        Vector2 probeCenter = worldPosition + Vector3.down * (stackHeight * 0.5f);
        Vector2 probeSize = new Vector2(stackSearchHalfWidth * 2f, stackHeight);
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(probeCenter, probeSize, 0f);

        float highestSupportTop = float.MinValue;
        for (int i = 0; i < overlaps.Length; i++)
        {
            Box otherBox = overlaps[i].GetComponentInParent<Box>();
            if (otherBox == null || otherBox == this || otherBox.IsHeld)
            {
                continue;
            }

            float top = overlaps[i].bounds.max.y;
            if (top > highestSupportTop && top <= worldPosition.y + 0.05f)
            {
                highestSupportTop = top;
            }
        }

        if (highestSupportTop > float.MinValue)
        {
            worldPosition.y = highestSupportTop + (stackHeight * 0.5f);
        }

        return worldPosition;
    }
}
