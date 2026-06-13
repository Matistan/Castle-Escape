using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CastleCollectible : MonoBehaviour
{
    private Collider2D collectibleCollider;
    private bool isCollected;

    private void Awake()
    {
        collectibleCollider = GetComponent<Collider2D>();
        collectibleCollider.isTrigger = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null && collectibleCollider is CircleCollider2D circleCollider)
        {
            float radius = Mathf.Max(spriteRenderer.sprite.bounds.extents.x, spriteRenderer.sprite.bounds.extents.y);
            circleCollider.radius = radius;
        }
        else if (collectibleCollider is CircleCollider2D fallbackCollider)
        {
            fallbackCollider.radius = 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || other.GetComponentInParent<CastlePlayerMovement>() == null)
        {
            return;
        }

        isCollected = true;
        if (CastleLevelManager.Instance != null)
        {
            CastleLevelManager.Instance.CollectPickup();
        }

        gameObject.SetActive(false);
    }
}
