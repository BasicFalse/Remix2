using UnityEngine;

public enum SkyboxTriggerType
{
    Toggle,
    Swap,
    Override,
    Original
}

public class SkyboxTrigger : MonoBehaviour
{
    public SkyboxTriggerType type;
    public bool ALPHA;
    public Material BETA;

    public void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case SkyboxTriggerType.Toggle:
                MiscGrabs.grab.SkyManager.ToggleSkybox(ALPHA);
                break;
            case SkyboxTriggerType.Swap:
                MiscGrabs.grab.SkyManager.ChangeSkybox(BETA);
                break;
            case SkyboxTriggerType.Override:
                MiscGrabs.grab.SkyManager.SetOverrideSky(ALPHA, BETA);
                break;
            case SkyboxTriggerType.Original:
                MiscGrabs.grab.SkyManager.SetOverrideSky(false);
                break;
        }
    }
}
