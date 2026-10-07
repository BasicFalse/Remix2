using UnityEngine;

[ExecuteAlways]
public class TargetRotatedWaveDistortionController : MonoBehaviour
{
    [Header("Renderer Target")]
    public Renderer targetRenderer;
    [Tooltip("Material slot index on the renderer")]
    public int materialIndex = 0;

    [Header("Wave Transforms")]
    public Transform waveOrigin;
    public Transform rotationTarget;

    [Header("Wave Settings")]
    public float amplitude = 0.05f;
    public float frequency = 6f;
    public float speed = 1f;

    static readonly int WaveOriginID = Shader.PropertyToID("_WaveOrigin");
    static readonly int TargetRightID = Shader.PropertyToID("_TargetRight");
    static readonly int TargetUpID = Shader.PropertyToID("_TargetUp");
    static readonly int AmpID = Shader.PropertyToID("_WaveAmplitude");
    static readonly int FreqID = Shader.PropertyToID("_WaveFrequency");
    static readonly int SpeedID = Shader.PropertyToID("_WaveSpeed");

    MaterialPropertyBlock _mpb;

    void OnEnable()
    {
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (!targetRenderer || !rotationTarget)
            return;

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        // Read existing properties so we don't wipe others
        targetRenderer.GetPropertyBlock(_mpb, materialIndex);

        if (waveOrigin)
            _mpb.SetVector(WaveOriginID, waveOrigin.position);

        _mpb.SetVector(TargetRightID, rotationTarget.right);
        _mpb.SetVector(TargetUpID, rotationTarget.up);

        _mpb.SetFloat(AmpID, amplitude);
        _mpb.SetFloat(FreqID, frequency);
        _mpb.SetFloat(SpeedID, speed);

        targetRenderer.SetPropertyBlock(_mpb, materialIndex);
    }
}
