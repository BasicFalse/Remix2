using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGlitchDeterministic : MonoBehaviour
{
    public Material glitchMaterial;

    [Header("Blocks")]
    [Range(0.005f, 0.5f)] public float blockSizeMax = 0.05f;
    [Range(0f, 1f)] public float blockJumpChanceMax = 0.5f;
    [Range(0f, 1f)] public float jumpStrengthMax = 0.4f;

    [Header("Color Bleed")]
    [Range(0f, 1f)] public float bleedStrength = 0.4f;
    [Range(0f, 0.02f)] public float bleedRadius = 0.005f;

    [Header("Random")]
    public float seed = 1f;

    // Call this manually if you want a new random layout
    public void Reseed()
    {
        seed = Random.value * 1000f;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!glitchMaterial)
        {
            Graphics.Blit(src, dest);
            return;
        }

        glitchMaterial.SetFloat("_BlockSize", Random.Range(0.005f, blockSizeMax));
        glitchMaterial.SetFloat("_JumpChance", Random.Range(0f, blockJumpChanceMax));
        glitchMaterial.SetFloat("_JumpStrength", Random.Range(0f, jumpStrengthMax));
        glitchMaterial.SetFloat("_BleedStrength", bleedStrength);
        glitchMaterial.SetFloat("_BleedRadius", bleedRadius);
        glitchMaterial.SetFloat("_Seed", seed);

        Graphics.Blit(src, dest, glitchMaterial);
    }
}
