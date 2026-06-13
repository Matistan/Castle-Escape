using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LevelBackground : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float padding = 1f;

    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -20;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Start()
    {
        FitToCamera();
    }

    private void FitToCamera()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null || targetCamera == null || !targetCamera.orthographic)
        {
            return;
        }

        float viewHeight = targetCamera.orthographicSize * 2f + padding;
        float viewWidth = viewHeight * targetCamera.aspect;
        spriteRenderer.size = new Vector2(viewWidth, viewHeight);
        transform.position = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y, 10f);
        transform.localScale = Vector3.one;
    }
}
