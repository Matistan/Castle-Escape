using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null && LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterPlayerAtExit(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null && LevelManager.Instance != null)
        {
            LevelManager.Instance.UnregisterPlayerAtExit(player);
        }
    }
}
