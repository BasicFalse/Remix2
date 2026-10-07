using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public MeshRenderer SBR;
    public Material currentSky;
    public bool overrideoriginal;
    public bool startEnabled;
    public bool overrideSkybox;
    public Material overrideSky;

    private bool goActive;
    private Material goMaterial;

    public void Awake()
    {
        if(overrideoriginal)
        UpdateSky();
        LogOriginalState();

        if(startEnabled) EnableSkybox();
        else DisableSkybox();
    }

    public void DisableSkybox() => SBR.gameObject.SetActive(false);
    public void EnableSkybox() => SBR.gameObject.SetActive(true);
    public void ToggleSkybox(bool flick) => SBR.gameObject.SetActive(flick);

    public void ChangeSkybox(Material mat)
    {
        currentSky = mat;
        UpdateSky();
    }

    public void SetOverrideSky(bool setOverride, Material mat = null)
    {
        if (setOverride)
        {
            overrideSkybox = true;
            overrideSky = mat;
        }
        else
        {
            overrideSkybox = false;
        }
        UpdateSky();
    }

    private void UpdateSky()
    {
        if (overrideSkybox)
        {
            SBR.material = overrideSky;
        }
        else
        {
            SBR.material = currentSky;
        }
    }

    public void ReturnToOriginalState(bool force)
    {
        if (force || !overrideSkybox)
        {
            SBR.gameObject.SetActive(goActive);
            SBR.material = goMaterial;
        }
    }

    private void LogOriginalState()
    {
        goActive = SBR.gameObject.activeSelf;
        goMaterial = SBR.material;
    }
}
