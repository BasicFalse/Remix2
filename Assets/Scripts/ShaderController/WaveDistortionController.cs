using UnityEngine;

public class WaveDistortionController : MonoBehaviour
{
    [Header("Material")]
    public Material waveMaterial;

    [Header("Wave Settings")]
    public float speed = 10f;
    public float maxRadius = 50f;
    public float startRadius = 0f;

    [Header("Auto Trigger")]
    public bool playOnStart = true;

    float currentRadius;
    bool playing;

    static readonly int WaveOriginID  = Shader.PropertyToID("_WaveOrigin");
    static readonly int WaveRadiusID  = Shader.PropertyToID("_WaveRadius");

    void Start()
    {
        if (playOnStart)
            Play();
    }

    void Update()
    {
        if (!playing || waveMaterial == null)
            return;

        currentRadius += speed * Time.deltaTime;
        waveMaterial.SetFloat(WaveRadiusID, currentRadius);

        if (currentRadius >= maxRadius)
            Stop();
    }

    public void Play()
    {
        if (waveMaterial == null)
            return;

        currentRadius = startRadius;
        playing = true;

        waveMaterial.SetVector(WaveOriginID, transform.position);
        waveMaterial.SetFloat(WaveRadiusID, currentRadius);
    }

    public void Stop()
    {
        playing = false;
    }

    // Call this to trigger the wave at any position
    public void PlayAt(Vector3 worldPosition)
    {
        if (waveMaterial == null)
            return;

        transform.position = worldPosition;
        Play();
    }
}
