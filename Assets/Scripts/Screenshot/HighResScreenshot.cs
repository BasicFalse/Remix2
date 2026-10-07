using UnityEngine;
using UnityEngine.InputSystem;

public class HighResScreenshot : MonoBehaviour
{
    public int resolutionMultiplier = 4; // Set higher multiplier for higher resolution
    public string fileName = "Screenshot.png";

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Player.TakeScreenshot.performed += ctx => TakeScreenshot();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void TakeScreenshot()
    {
        string path = Application.dataPath + "/" + fileName;
        ScreenCapture.CaptureScreenshot(path, resolutionMultiplier);
        Debug.Log($"Screenshot saved to: {path}");
    }
}