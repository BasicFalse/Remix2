using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class EarthquakeMeshDeformer : MonoBehaviour
{
    public enum DeformMode { Shake, Shockwave, Crack }
    public DeformMode mode = DeformMode.Shake;

    public float duration = 2f;
    public float magnitude = 0.5f;
    public float speed = 5f;

    public Transform epicenter;
    public Vector3 epicenterV3 = Vector3.zero;
    public float shockwaveRadius = 10f;
    public Vector3 faultLineDirection = Vector3.right;

    [Header("Crack Settings")]
    public float crackSpeed = 4f;
    public float crackLength = 10f;
    public float crackDepth = 0.4f;

    [Range(0f, 1f)] public float crackJitter = 0.2f; // adds noisy jaggedness
    [Range(0f, 2f)] public float falloffX = 1f; // falloff along crack
    [Range(0f, 2f)] public float falloffY = 1f; // falloff perpendicular


    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;
    private float elapsed;

    private bool isQuaking;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        epicenterV3 = epicenter.position;
        //faultLineDirection = epicenter.rotation.eulerAngles;

        Mesh mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];
        System.Array.Copy(originalVertices, deformedVertices, originalVertices.Length);
    }

    void Update()
    {
        if (isQuaking)
        {
            elapsed += Time.deltaTime;
            ApplyDeformation();

            if (elapsed >= duration)
            {
                isQuaking = false;
            }
        }
    }


    public void TriggerQuake()
    {
        elapsed = 0f;
        isQuaking = true;
    }

    void ApplyDeformation()
    {
        Mesh mesh = meshFilter.mesh;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 localVertex = originalVertices[i];
            Vector3 worldVertex = transform.TransformPoint(localVertex);

            float offsetY = 0f;

            switch (mode)
            {
                case DeformMode.Shake:
                    offsetY = Mathf.PerlinNoise(worldVertex.x * 0.5f + Time.time * speed, worldVertex.z * 0.5f + Time.time * speed) * magnitude;
                    break;

                case DeformMode.Shockwave:
                    float dist = Vector3.Distance(worldVertex, epicenterV3);
                    float falloff = Mathf.Clamp01(1f - dist / shockwaveRadius);
                    offsetY = Mathf.Sin(Time.time * speed - dist) * falloff * magnitude;
                    break;

                case DeformMode.Crack:
                {
                    Vector3 worldDown = Vector3.down;
                    Vector3 faultDir = faultLineDirection.normalized;

                    float crackFront = Mathf.Min(elapsed * crackSpeed, crackLength);
                    float distX = Vector3.Dot(worldVertex - epicenterV3, faultDir);

                    if (Mathf.Abs(distX) <= crackFront)
                    {
                        // Optional: jaggedness (add noise to make it look more natural)
                        float noise = crackJitter > 0 ? Mathf.PerlinNoise(worldVertex.x * 10f, worldVertex.z * 10f) : 1f;

                        // Sharp falloff only along crack direction
                        float falloff2 = Mathf.Clamp01(1f - Mathf.Abs(distX) / (crackFront * falloffX));

                        Vector3 offsetWorld = worldDown * crackDepth * falloff2 * noise;
                        Vector3 offsetLocal = transform.InverseTransformDirection(offsetWorld);
                        deformedVertices[i] += offsetLocal;
                    }

                    continue;
                }



            }

            deformedVertices[i] = localVertex + Vector3.up * offsetY;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = null; // force update
        meshCollider.sharedMesh = mesh;
    }

    void ResetMesh()
    {
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = originalVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}
