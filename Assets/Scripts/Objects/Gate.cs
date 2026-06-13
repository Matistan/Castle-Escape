using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Gate : MonoBehaviour
{
    [SerializeField] private Color gateColor = new Color(0.45f, 0.32f, 0.22f, 1f);

    private void Reset()
    {
        ApplyVisualDefaults();
    }

    private void Awake()
    {
        ApplyVisualDefaults();
        SpriteColliderUtility.FitBoxColliderToSprite(GetComponent<BoxCollider2D>(), GetComponent<SpriteRenderer>());
    }

    private void ApplyVisualDefaults()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = gateColor;
        spriteRenderer.sortingOrder = 5;
    }
}
