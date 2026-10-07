using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class ProceduralSound : MonoBehaviour
{
    public enum SoundType
    {
        SineTone,
        WhiteNoise,
        Engine,
        Laser,
        Wind,
        Ambient
    }

    public enum WindStyle
    {
        LightBreeze,
        Storm,
        Tunnel
    }

    [Header("Sound Type")]
    public SoundType soundType = SoundType.SineTone;

    [Header("General Settings")]
    public float baseFrequency = 440f;
    public float amplitude = 0.1f;

    [Header("Wind Settings")]
    public WindStyle windStyle = WindStyle.LightBreeze;
    public Transform listener;
    public Rigidbody playerRigidbody;
    public float windRadius = 20f;

    private float sampleRate;
    private float phase;
    private float engineTime;
    private float laserTime;
    private float ambientTargetFreq;

    // Wind internals
    private float windFilter;
    private float windGust;
    private float windGustTarget;
    private float windPan;

    // Cached main-thread values
    private float cachedSpeed;
    private float cachedDistanceFalloff = 1f;

    private System.Random random;

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        random = new System.Random();

        var source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = true;
        source.spatialBlend = 0f;
        source.Play();

        ambientTargetFreq = baseFrequency;
    }

    void Update()
    {
        // Cache speed safely
        if (playerRigidbody != null)
            cachedSpeed = playerRigidbody.linearVelocity.magnitude;
        else
            cachedSpeed = 0f;

        // Cache distance safely
        if (listener != null)
        {
            float dist = Vector3.Distance(transform.position, listener.position);
            cachedDistanceFalloff = Mathf.Clamp01(1f - (dist / windRadius));
        }
        else
        {
            cachedDistanceFalloff = 1f;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0f;

            switch (soundType)
            {
                case SoundType.SineTone:
                    sample = GenerateSine(baseFrequency);
                    break;

                case SoundType.WhiteNoise:
                    sample = (float)(random.NextDouble() * 2.0 - 1.0);
                    break;

                case SoundType.Engine:
                    engineTime += 1f / sampleRate;
                    float engineFreq = baseFrequency + Mathf.Sin(engineTime * 2f) * 20f;
                    sample = GenerateSine(engineFreq) * 0.8f;
                    break;

                case SoundType.Laser:
                    laserTime += 1f / sampleRate;
                    float laserFreq = Mathf.Lerp(baseFrequency * 4f, baseFrequency, laserTime);
                    sample = GenerateSine(laserFreq);
                    if (laserTime > 0.5f) laserTime = 0f;
                    break;

                case SoundType.Ambient:
                    float target = (float)(random.NextDouble() * baseFrequency);
                    ambientTargetFreq += (target - ambientTargetFreq) * 0.0005f;
                    sample = GenerateSine(ambientTargetFreq);
                    break;

                case SoundType.Wind:
                    sample = GenerateWind(channels, data, i);
                    continue;
            }

            sample *= amplitude;

            for (int c = 0; c < channels; c++)
                data[i + c] = sample;
        }
    }

    float GenerateWind(int channels, float[] data, int i)
    {
        float baseNoise = (float)(random.NextDouble() * 2.0 - 1.0);

        float filterStrength = 0.02f;
        float gustSpeed = 0.0005f;
        float gustAmount = 0.3f;
        float volume = 0.4f;

        switch (windStyle)
        {
            case WindStyle.LightBreeze:
                filterStrength = 0.01f;
                gustSpeed = 0.0002f;
                gustAmount = 0.15f;
                volume = 0.25f;
                break;

            case WindStyle.Storm:
                filterStrength = 0.05f;
                gustSpeed = 0.002f;
                gustAmount = 0.6f;
                volume = 0.8f;
                break;

            case WindStyle.Tunnel:
                filterStrength = 0.03f;
                gustSpeed = 0.001f;
                gustAmount = 0.4f;
                volume = 0.5f;
                break;
        }

        // Low-pass filter
        windFilter += (baseNoise - windFilter) * filterStrength;

        // Gust modulation
        windGustTarget += (float)(random.NextDouble() * 2.0 - 1.0) * gustSpeed;
        windGustTarget = Mathf.Clamp(windGustTarget, -1f, 1f);
        windGust += (windGustTarget - windGust) * 0.001f;

        float speedBoost = 1f + cachedSpeed * 0.05f;
        float distanceFalloff = cachedDistanceFalloff;

        float finalSample = windFilter *
                            (volume + windGust * gustAmount) *
                            speedBoost *
                            distanceFalloff *
                            amplitude;

        if (windStyle == WindStyle.Tunnel && channels == 2)
        {
            windPan += 0.0005f;
            float pan = Mathf.Sin(windPan);

            data[i] = finalSample * (0.5f - pan * 0.5f);
            data[i + 1] = finalSample * (0.5f + pan * 0.5f);
            return 0f;
        }

        return finalSample;
    }

    float GenerateSine(float freq)
    {
        float increment = freq * 2f * Mathf.PI / sampleRate;
        phase += increment;

        if (phase > Mathf.PI * 2f)
            phase -= Mathf.PI * 2f;

        return Mathf.Sin(phase);
    }
}