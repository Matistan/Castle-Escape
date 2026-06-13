using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CastleCarryableBoxAudio : MonoBehaviour
{
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float dropVolume = 0.9f;

    private Rigidbody2D body;
    private RigidbodyType2D previousBodyType;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        previousBodyType = body != null ? body.bodyType : RigidbodyType2D.Dynamic;
    }

    private void LateUpdate()
    {
        if (body == null)
        {
            return;
        }

        RigidbodyType2D currentBodyType = body.bodyType;
        if (currentBodyType == previousBodyType)
        {
            return;
        }

        if (previousBodyType == RigidbodyType2D.Dynamic && currentBodyType == RigidbodyType2D.Kinematic && pickupClip != null)
        {
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);
        }
        else if (previousBodyType == RigidbodyType2D.Kinematic && currentBodyType == RigidbodyType2D.Dynamic && dropClip != null)
        {
            AudioSource.PlayClipAtPoint(dropClip, transform.position, dropVolume);
        }

        previousBodyType = currentBodyType;
    }
}