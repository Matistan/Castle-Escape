using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour
{
    [SerializeField] private bool startOn;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite middleSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private float frameTime = 0.06f;
    [SerializeField] private float interactRadius = 1.25f;
    [Header("Controlled Objects")]
    [SerializeField] private GameObject[] controlledObjects;
    [SerializeField] private bool hideControlledWhenActive = true;
    [Header("Audio")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField, Range(0f, 1f)] private float activateVolume = 0.8f;

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
            SpriteColliderUtility.FitBoxColliderToSprite(boxCollider, spriteRenderer);
        }

        SetStateImmediate(startOn);
    }

    public static bool TryFindNearestInRange(Vector3 position, float radius, out Lever nearestLever)
    {
        Lever[] levers = FindObjectsByType<Lever>(FindObjectsSortMode.None);
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

        ApplyControlledObjects();
        PlayActivateSound();
    }

    private void SetStateImmediate(bool active)
    {
        isOn = active;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
        ApplyControlledObjects();
    }

    private void ApplyControlledObjects()
    {
        if (controlledObjects == null)
        {
            return;
        }

        bool visible = hideControlledWhenActive ? !isOn : isOn;
        for (int i = 0; i < controlledObjects.Length; i++)
        {
            if (controlledObjects[i] != null)
            {
                controlledObjects[i].SetActive(visible);
            }
        }
    }

    private void PlayActivateSound()
    {
        if (activateClip != null)
        {
            SettingsManager.PlaySfx(activateClip, transform.position, activateVolume);
        }
    }
}
