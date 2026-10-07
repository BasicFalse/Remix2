using UnityEngine;

public class FogSetter : RemixScript
{
    public bool enableFog;
    public Color col;
    public FogMode mode;
    public float fogStart;
    public float fogEnd;
    public float fogDensity;

    private void Awake()
    {
        MeshRenderer mr; TryGetComponent(out mr);
        if(mr != null){ mr.enabled = false; }
    }

    [ContextMenu("OnTriggerEnter")]
    private void OnTriggerEnter(Collider other)
    {
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = col;
            RenderSettings.fogMode = mode;
            if (mode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = fogStart;
                RenderSettings.fogEndDistance = fogEnd;
            }
            else
            {
                RenderSettings.fogDensity = fogDensity;
            }
        }
        else { RenderSettings.fog = false; }
        
    }
}
