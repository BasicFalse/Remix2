using UnityEngine;

public class StalkSkin : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // Reference to the skinned mesh renderer
    public int materialIndexToChange = 0;           // Index of the material you want to change
    public float colorMin = 0f;                     // Minimum color value (0 = black)
    public float colorMax = 0.5f;                   // Maximum color value (0.5 = dark color)

    private void Start()
    {
        // Set a random dark color at the start
        SetRandomMaterialColor();
    }

    public void SetRandomMaterialColor()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer is not assigned.");
            return;
        }

        // Make sure the material index is valid
        if (materialIndexToChange < 0 || materialIndexToChange >= skinnedMeshRenderer.materials.Length)
        {
            Debug.LogError("Material index out of range.");
            return;
        }

        // Generate random values for R, G, and B within the dark color range
        float r = Random.Range(colorMin, colorMax);
        float g = Random.Range(colorMin, colorMax);
        float b = Random.Range(colorMin, colorMax);

        // Create a color using the dark values for R, G, and B
        Color darkColor = new Color(r, g, b);

        // Get the materials array
        Material[] materials = skinnedMeshRenderer.materials;

        // Change the color of the specified material
        materials[materialIndexToChange].color = darkColor;

        // Assign the modified materials array back to the renderer
        skinnedMeshRenderer.materials = materials;
    }
}