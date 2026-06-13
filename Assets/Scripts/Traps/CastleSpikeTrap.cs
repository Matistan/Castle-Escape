using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CastleSpikeTrap : MonoBehaviour
{
    [SerializeField] private bool startArmed = true;

    private Collider2D trapCollider;
    private bool isArmed;

    public bool IsArmed => isArmed;

    private void Awake()
    {
        trapCollider = GetComponent<Collider2D>();
        trapCollider.isTrigger = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && trapCollider is BoxCollider2D boxCollider)
        {
            CastleSpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }

        isArmed = startArmed;
        UpdateArmedState();
    }

    private void OnEnable()
    {
        if (trapCollider == null)
        {
            trapCollider = GetComponent<Collider2D>();
        }

        UpdateArmedState();
    }

    public void SetArmed(bool armed)
    {
        isArmed = armed;
        UpdateArmedState();
    }

    private void UpdateArmedState()
    {
        if (trapCollider != null)
        {
            trapCollider.enabled = isArmed;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = isArmed ? 1f : 0.35f;
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
        if (!isArmed || other.GetComponentInParent<CastlePlayerMovement>() == null)
        {
            return;
        }

        CastleLevelReset.RequestReset();
    }
}
