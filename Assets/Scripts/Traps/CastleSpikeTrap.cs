using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class CastleSpikeTrap : MonoBehaviour
{
    private Collider2D trapCollider;
    private bool isResetting;

    private void Awake()
    {
        trapCollider = GetComponent<Collider2D>();
        trapCollider.isTrigger = true;
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
        if (isResetting)
        {
            return;
        }

        if (other.GetComponentInParent<CastlePlayerMovement>() == null)
        {
            return;
        }

        isResetting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}