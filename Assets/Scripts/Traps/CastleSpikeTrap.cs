using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CastleSpikeTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    private void Awake()
    {
        trapCollider = GetComponent<Collider2D>();
        trapCollider.isTrigger = true;
        trapCollider.enabled = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && trapCollider is BoxCollider2D boxCollider)
        {
            CastleSpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryResetLevel(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryResetLevel(other);
    }

    private void TryResetLevel(Collider2D other)
    {
        if (other.GetComponentInParent<CastlePlayerMovement>() == null)
        {
            return;
        }

        CastleLevelReset.RequestReset();
    }
}
