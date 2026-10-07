using UnityEngine;

public class UnlitCamera : MonoBehaviour
{
    public Shader replacementShader;

    private Camera cam;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();

        if (replacementShader)
            cam.SetReplacementShader(replacementShader, "");
    }

    private void OnDisable()
    {
        if (cam)
            cam.ResetReplacementShader();
    }
}
