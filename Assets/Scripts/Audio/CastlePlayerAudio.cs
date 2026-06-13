using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CastlePlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip jumpClip;
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.85f;
    [SerializeField] private float jumpVelocityThreshold = 0.1f;

    private Rigidbody2D body;
    private bool playedForCurrentAirborneState;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (body == null || jumpClip == null)
        {
            return;
        }

        if (body.linearVelocity.y <= jumpVelocityThreshold)
        {
            playedForCurrentAirborneState = false;
            return;
        }

        if (playedForCurrentAirborneState)
        {
            return;
        }

        playedForCurrentAirborneState = true;
        SettingsManager.PlaySfx(jumpClip, transform.position, jumpVolume);
    }
}