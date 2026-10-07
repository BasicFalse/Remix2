using UnityEngine;

public class NametagOcclusionFade : MonoBehaviour
{
    public Transform target;            // Where the nametag sits (usually head)
    public Renderer[] renderers;        // The nametag renderers (TMP text, mesh billboard, etc.)
    public float maxOpacity = 1f;
    public float fadeSpeed = 10f;
    public LayerMask occlusionMask;     // What counts as an occluder (Default, Environment, etc.)

    private float currentOpacity;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        currentOpacity = maxOpacity;

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (cam == null || target == null) return;

        Vector3 camPos = cam.transform.position;
        Vector3 toTarget = (target.position - camPos);
        float distance = toTarget.magnitude;

        // Raycast to check if something is blocking the view
        bool blocked = Physics.Raycast(
            camPos,
            toTarget.normalized,
            distance,
            occlusionMask,
            QueryTriggerInteraction.Ignore
        );

        float targetOpacity = blocked ? 0f : maxOpacity;

        // Smooth fade
        currentOpacity = Mathf.Lerp(currentOpacity, targetOpacity, Time.deltaTime * fadeSpeed);

        ApplyOpacity(currentOpacity);
    }

    void ApplyOpacity(float alpha)
    {
        foreach (var r in renderers)
        {
            if (!r) continue;

            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }
    }
}
