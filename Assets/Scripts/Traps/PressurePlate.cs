using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite pressedSprite;
    [Header("Controlled Objects")]
    [SerializeField] private GameObject[] controlledObjects;
    [SerializeField] private bool hideControlledWhenActive = true;
    [Header("Audio")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField, Range(0f, 1f)] private float activateVolume = 0.8f;

    private readonly HashSet<Collider2D> occupants = new HashSet<Collider2D>();
    private SpriteRenderer spriteRenderer;
    private bool isPressed;

    public bool IsPressed => isPressed;

    private void Awake()
    {
        EnsureInitialized();

        ApplyState(false, playSound: false);
    }

    private void EnsureInitialized()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActivator(other) || !occupants.Add(other))
        {
            return;
        }

        UpdateState();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsActivator(other))
        {
            return;
        }

        if (occupants.Add(other))
        {
            UpdateState();
        }
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
        if (other == null || other.isTrigger)
        {
            return false;
        }

        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            return true;
        }

        Box box = other.GetComponentInParent<Box>();
        return box != null && !box.IsHeld;
    }

    private void UpdateState()
    {
        ApplyState(occupants.Count > 0, playSound: true);
    }

    private void ApplyState(bool pressed, bool playSound)
    {
        EnsureInitialized();
        bool stateChanged = pressed != isPressed;
        isPressed = pressed;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isPressed ? pressedSprite : defaultSprite;
        }

        ApplyControlledObjects();

        if (playSound && stateChanged && pressed && activateClip != null)
        {
            SettingsManager.PlaySfx(activateClip, transform.position, activateVolume);
        }
    }

    private void ApplyControlledObjects()
    {
        if (controlledObjects == null)
        {
            return;
        }

        bool visible = hideControlledWhenActive ? !isPressed : isPressed;
        for (int i = 0; i < controlledObjects.Length; i++)
        {
            if (controlledObjects[i] != null)
            {
                controlledObjects[i].SetActive(visible);
            }
        }
    }
}
