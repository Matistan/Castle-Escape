using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CastlePressurePlate : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private CastleTrapLink[] trapLinks;

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

        if (plateCollider is BoxCollider2D boxCollider)
        {
            CastleSpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }

        ApplyState(false);
    }

    public void ApplyLinks(params CastleTrapLink[] links)
    {
        trapLinks = links;
        ApplyState(isPressed);
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

        if (other.GetComponentInParent<CastlePlayerMovement>() != null)
        {
            return true;
        }

        CastleCarryableBox box = other.GetComponentInParent<CastleCarryableBox>();
        return box != null && !box.IsHeld;
    }

    private void UpdateState()
    {
        ApplyState(occupants.Count > 0);
    }

    private void ApplyState(bool pressed)
    {
        isPressed = pressed;
        spriteRenderer.sprite = isPressed ? pressedSprite : defaultSprite;

        if (trapLinks == null)
        {
            return;
        }

        for (int i = 0; i < trapLinks.Length; i++)
        {
            ApplyLink(trapLinks[i], isPressed);
        }
    }

    private static void ApplyLink(CastleTrapLink link, bool sourceActive)
    {
        if (link.target == null)
        {
            return;
        }

        bool targetActive = link.invertActiveState ? !sourceActive : sourceActive;
        link.target.SetActive(targetActive);
        ApplyLinkedComponentStates(link.target, targetActive);
    }

    private static void ApplyLinkedComponentStates(GameObject linkedObject, bool targetActive)
    {
        CastleSpikeTrap spikeTrap = linkedObject.GetComponent<CastleSpikeTrap>();
        if (spikeTrap != null)
        {
            spikeTrap.SetArmed(targetActive);
        }
    }
}
