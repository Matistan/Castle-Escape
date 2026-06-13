using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CastlePressurePlate : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private GameObject[] linkedObjects;

    private readonly HashSet<Collider2D> occupants = new HashSet<Collider2D>();
    private Collider2D plateCollider;
    private SpriteRenderer spriteRenderer;
    private bool isPressed;

    public bool IsPressed => isPressed;

    private void Awake()
    {
        plateCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        plateCollider.isTrigger = true;
        ApplyState(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActivator(other) || !occupants.Add(other))
        {
            return;
        }

        UpdateState();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!occupants.Remove(other))
        {
            return;
        }

        UpdateState();
    }

    private bool IsActivator(Collider2D other)
    {
        if (other == null || other.attachedRigidbody == null || other.isTrigger)
        {
            return false;
        }

        return other.GetComponentInParent<CastlePlayerMovement>() != null
            || other.GetComponentInParent<CastleCarryableBox>() != null;
    }

    private void UpdateState()
    {
        ApplyState(occupants.Count > 0);
    }

    private void ApplyState(bool pressed)
    {
        isPressed = pressed;
        spriteRenderer.sprite = isPressed ? pressedSprite : defaultSprite;

        if (linkedObjects == null)
        {
            return;
        }

        for (int i = 0; i < linkedObjects.Length; i++)
        {
            if (linkedObjects[i] != null)
            {
                linkedObjects[i].SetActive(isPressed);
            }
        }
    }
}
