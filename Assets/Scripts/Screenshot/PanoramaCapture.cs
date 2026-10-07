using System.IO;
using UnityEngine;

public class PanoramaCapture : MonoBehaviour
{
    public int panoramaWidth = 8192; // Width of the final panoramic image
    public int panoramaHeight = 1024; // Height of the final panoramic image
    public int segments = 36; // Number of horizontal segments
    public string outputFileName = "Panorama.png";

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private Texture2D panoramaTexture;

    void Start()
    {
        // Initialize render camera and textures
        renderCamera = GetComponent<Camera>();
        if (renderCamera == null)
        {
            Debug.LogError("Attach this script to a GameObject with a Camera component.");
            return;
        }

        int segmentWidth = panoramaWidth / segments;
        renderTexture = new RenderTexture(segmentWidth, panoramaHeight, 24);
        panoramaTexture = new Texture2D(panoramaWidth, panoramaHeight, TextureFormat.RGB24, false);

        renderCamera.targetTexture = renderTexture;
    }

    [ContextMenu("Take Panoramic Screenshot")]
    public void TakePanoramicScreenshot()
    {
        if (renderCamera == null)
        {
            Debug.LogError("No camera found to render the panorama.");
            return;
        }

        StartCoroutine(CapturePanorama());
    }

    private System.Collections.IEnumerator CapturePanorama()
    {
        float stepAngle = 360f / segments;
        Quaternion originalRotation = transform.rotation;

        // Initialize the panorama texture with black
        panoramaTexture = new Texture2D(renderTexture.width * segments, renderTexture.height);
        Color[] blackPixels = new Color[panoramaTexture.width * panoramaTexture.height];
        for (int i = 0; i < blackPixels.Length; i++) blackPixels[i] = Color.black;
        panoramaTexture.SetPixels(blackPixels);
        panoramaTexture.Apply();

        for (int i = 0; i < segments; i++)
        {
            // Rotate the camera
            transform.rotation = originalRotation * Quaternion.Euler(0, stepAngle * i, 0);

            // Wait for stabilization
            yield return new WaitForEndOfFrame();

            // Capture the segment
            RenderTexture.active = renderTexture;
            Texture2D segmentTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            segmentTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            segmentTexture.Apply();

            // Copy pixels into the panorama texture
            panoramaTexture.SetPixels(i * renderTexture.width, 0, renderTexture.width, renderTexture.height, segmentTexture.GetPixels());

            Destroy(segmentTexture);
        }

        // Restore the original rotation
        transform.rotation = originalRotation;

        // Save the panorama
        panoramaTexture.Apply();
        byte[] bytes = panoramaTexture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, outputFileName), bytes);

        Debug.Log($"Panorama saved to {Path.Combine(Application.dataPath, outputFileName)}");
    }
}