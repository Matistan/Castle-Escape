using UnityEngine;

public class CastleTrapLinkSetup : MonoBehaviour
{
    [Header("Optional overrides. Leave empty to find objects by name.")]
    [SerializeField] private CastleLever lever;
    [SerializeField] private GameObject leverGate;
    [SerializeField] private CastlePressurePlate pressurePlate;
    [SerializeField] private GameObject plateGate;
    [SerializeField] private CastleSpikeTrap plateSpikeTrap;

    private void Awake()
    {
        lever ??= FindSceneObject<CastleLever>("LeverSwitch");
        leverGate ??= GameObject.Find("LeverGate");
        pressurePlate ??= FindSceneObject<CastlePressurePlate>("PressurePlate");
        plateGate ??= GameObject.Find("PlateGate");
        plateSpikeTrap ??= FindSceneObject<CastleSpikeTrap>("PlateSpikeTrap");

        if (lever != null && leverGate != null)
        {
            lever.ApplyLinks(new CastleTrapLink
            {
                target = leverGate,
                invertActiveState = true
            });
        }

        if (pressurePlate != null)
        {
            if (plateGate != null && plateSpikeTrap != null)
            {
                pressurePlate.ApplyLinks(
                    new CastleTrapLink { target = plateGate, invertActiveState = true },
                    new CastleTrapLink { target = plateSpikeTrap.gameObject, invertActiveState = false });
                plateSpikeTrap.SetArmed(false);
            }
            else if (plateGate != null)
            {
                pressurePlate.ApplyLinks(new CastleTrapLink
                {
                    target = plateGate,
                    invertActiveState = true
                });
            }
            else if (plateSpikeTrap != null)
            {
                pressurePlate.ApplyLinks(new CastleTrapLink
                {
                    target = plateSpikeTrap.gameObject,
                    invertActiveState = false
                });
                plateSpikeTrap.SetArmed(false);
            }
        }
    }

    private static T FindSceneObject<T>(string objectName) where T : Component
    {
        GameObject sceneObject = GameObject.Find(objectName);
        return sceneObject != null ? sceneObject.GetComponent<T>() : null;
    }
}
