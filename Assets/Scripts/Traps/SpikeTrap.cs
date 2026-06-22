using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeTrap : MonoBehaviour
{
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
        if (other.GetComponentInParent<PlayerMovement>() == null)
        {
            return;
        }

        LevelReset.RequestReset();
    }
}
