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
    [SerializeField] private GameObject[] linkedObjects;

    private Collider2D leverCollider;
    private SpriteRenderer spriteRenderer;
    private Coroutine animationRoutine;
    private bool isOn;

    public bool IsOn => isOn;

    private void Awake()
    {
        leverCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        leverCollider.isTrigger = true;
        SetStateImmediate(startOn);
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

    private IEnumerator AnimateState(bool active)
    {
        isOn = active;
        spriteRenderer.sprite = isOn ? offSprite : onSprite;
        yield return new WaitForSeconds(frameTime);
        spriteRenderer.sprite = middleSprite;
        yield return new WaitForSeconds(frameTime);
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
        if (linkedObjects == null)
        {
            return;
        }

        for (int i = 0; i < linkedObjects.Length; i++)
        {
            if (linkedObjects[i] != null)
            {
                linkedObjects[i].SetActive(isOn);
            }
        }
    }
}
