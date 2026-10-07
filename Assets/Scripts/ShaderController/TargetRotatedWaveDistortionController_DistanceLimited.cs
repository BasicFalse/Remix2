using UnityEngine;

[ExecuteAlways]
public class TargetRotatedWaveDistortionController_DistanceLimited : MonoBehaviour
{
    [Header("Renderer Target")]
    public Renderer targetRenderer;
    public int materialIndex = 0;

    [Header("Wave Transforms")]
    public Transform waveOrigin;
    public Transform rotationTarget;

    [Header("Wave Settings")]
    public float amplitude = 0.05f;
    public float frequency = 6f;
    public float speed = 1f;

    [Header("Distance Limits")]
    public float minDistance = 0f;
    public float maxDistance = 5f;

    static readonly int WaveOriginID   = Shader.PropertyToID("_WaveOrigin");
    static readonly int TargetRightID  = Shader.PropertyToID("_TargetRight");
    static readonly int TargetUpID     = Shader.PropertyToID("_TargetUp");
    static readonly int AmpID          = Shader.PropertyToID("_WaveAmplitude");
    static readonly int FreqID         = Shader.PropertyToID("_WaveFrequency");
    static readonly int SpeedID        = Shader.PropertyToID("_WaveSpeed");
    static readonly int MinDistID      = Shader.PropertyToID("_WaveMinDistance");
    static readonly int MaxDistID      = Shader.PropertyToID("_WaveMaxDistance");

    MaterialPropertyBlock _mpb;

    void OnEnable()
    {
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;
    
        if (!targetRenderer || !rotationTarget)
            return;
    
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
    
        targetRenderer.GetPropertyBlock(_mpb, materialIndex);
    
        if (waveOrigin)
            _mpb.SetVector("_WaveOrigin", waveOrigin.position);
    
        _mpb.SetVector("_TargetRight", rotationTarget.right);
        _mpb.SetVector("_TargetUp", rotationTarget.up);
    
        _mpb.SetFloat("_WaveAmplitude", amplitude);
        _mpb.SetFloat("_WaveFrequency", frequency);
        _mpb.SetFloat("_WaveSpeed", speed);
        _mpb.SetFloat("_WaveMinDistance", minDistance);
        _mpb.SetFloat("_WaveMaxDistance", maxDistance);
    
        targetRenderer.SetPropertyBlock(_mpb, materialIndex);
    }

}
