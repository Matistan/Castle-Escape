using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    private Collider2D exitCollider;

    private void Awake()
    {
        exitCollider = GetComponent<Collider2D>();
        exitCollider.isTrigger = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && exitCollider is BoxCollider2D boxCollider)
        {
            SpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null && LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterPlayerAtExit(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null && LevelManager.Instance != null)
        {
            LevelManager.Instance.UnregisterPlayerAtExit(player);
        }
    }
}
