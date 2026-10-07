using UnityEngine;

[ExecuteAlways]
public class ProximityEyeController : MonoBehaviour
{
    public Transform Target;
    public bool setEyeVals;
    public bool setEyeTargPos;
    public Renderer[] eyeRenderer;
    public float movementInfluence = 3f;
    public float scaleAmount = 1f;

    void Update()
    {
        foreach (Renderer r in eyeRenderer)
        {
            #if UNITY_EDITOR
            if (setEyeTargPos) { r.sharedMaterial.SetVector("_TargPos", Target.position); }
            if (setEyeVals) { r.sharedMaterial.SetFloat("_OpenDistance", movementInfluence); }
            if (setEyeVals) { r.sharedMaterial.SetFloat("_ScaleAmount", scaleAmount); }
            #else
            if (setEyeTargPos) { r.material.SetVector("_TargPos", Target.position); }
            if (setEyeVals) { r.material.SetFloat("_OpenDistance", movementInfluence); }
            if (setEyeVals) { r.material.SetFloat("_ScaleAmount", scaleAmount); }
            #endif
        }
    }
}
