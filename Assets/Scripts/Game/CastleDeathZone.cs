using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CastleDeathZone : MonoBehaviour
{
    private Collider2D deathCollider;

    private void Awake()
    {
        deathCollider = GetComponent<Collider2D>();
        deathCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<CastlePlayerMovement>() == null)
        {
            return;
        }

        CastleLevelReset.RequestReset();
    }
}
