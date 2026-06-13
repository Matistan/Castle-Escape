using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CastleLever : MonoBehaviour
{
    [SerializeField] private bool startOn;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite middleSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private float frameTime = 0.06f;
    [SerializeField] private float interactRadius = 1.25f;
    [SerializeField] private CastleTrapLink[] trapLinks;

    private Collider2D leverCollider;
    private SpriteRenderer spriteRenderer;
    private Coroutine animationRoutine;
    private bool isOn;

    public bool IsOn => isOn;
    public float InteractRadius => interactRadius;

    private void Awake()
    {
        leverCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        leverCollider.isTrigger = true;

        if (leverCollider is BoxCollider2D boxCollider)
        {
            CastleSpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }

        SetStateImmediate(startOn);
    }

    public static bool TryFindNearestInRange(Vector3 position, float radius, out CastleLever nearestLever)
    {
        CastleLever[] levers = FindObjectsByType<CastleLever>(FindObjectsSortMode.None);
        nearestLever = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < levers.Length; i++)
        {
            float distance = Vector2.Distance(position, levers[i].transform.position);
            float allowedRadius = Mathf.Max(radius, levers[i].interactRadius);
            if (distance > allowedRadius || distance >= nearestDistance)
            {
                continue;
            }

            nearestDistance = distance;
            nearestLever = levers[i];
        }

        return nearestLever != null;
    }

    public void ApplyLinks(params CastleTrapLink[] links)
    {
        trapLinks = links;
        ApplyLinkedObjects();
    }

    public void Toggle()
    {
        SetState(!isOn);
    }

    public void SetState(bool active)
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        animationRoutine = StartCoroutine(AnimateState(active));
    }

    private IEnumerator AnimateState(bool targetOn)
    {
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
        yield return new WaitForSeconds(frameTime);
        spriteRenderer.sprite = middleSprite;
        yield return new WaitForSeconds(frameTime);
        isOn = targetOn;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
        animationRoutine = null;

        ApplyLinkedObjects();
    }

    private void SetStateImmediate(bool active)
    {
        isOn = active;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
        ApplyLinkedObjects();
    }

    private void ApplyLinkedObjects()
    {
        if (trapLinks == null)
        {
            return;
        }

        for (int i = 0; i < trapLinks.Length; i++)
        {
            ApplyLink(trapLinks[i], isOn);
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
