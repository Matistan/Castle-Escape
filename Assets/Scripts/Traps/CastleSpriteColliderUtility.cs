using UnityEngine;

public static class CastleSpriteColliderUtility
{
    public static void FitBoxColliderToSprite(BoxCollider2D boxCollider, SpriteRenderer spriteRenderer, float padding = 0.02f)
    {
        if (boxCollider == null || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector2 spriteSize = spriteBounds.size;
        Vector2 spriteCenter = spriteBounds.center;
        boxCollider.size = spriteSize + (Vector2.one * padding * 2f);
        boxCollider.offset = spriteCenter;
    }
}
