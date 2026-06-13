using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CastleLevelExit : MonoBehaviour
{
    private Collider2D exitCollider;

    private void Awake()
    {
        exitCollider = GetComponent<Collider2D>();
        exitCollider.isTrigger = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && exitCollider is BoxCollider2D boxCollider)
        {
            CastleSpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CastlePlayerMovement player = other.GetComponentInParent<CastlePlayerMovement>();
        if (player != null && CastleLevelManager.Instance != null)
        {
            CastleLevelManager.Instance.RegisterPlayerAtExit(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CastlePlayerMovement player = other.GetComponentInParent<CastlePlayerMovement>();
        if (player != null && CastleLevelManager.Instance != null)
        {
            CastleLevelManager.Instance.UnregisterPlayerAtExit(player);
        }
    }
}
