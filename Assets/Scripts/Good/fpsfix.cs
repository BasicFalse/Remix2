using UnityEngine;
using UnityEngine.XR;

public class fpsfix : MonoBehaviour
{
    //public OVRManager OVRM;
    void Start()
    {
        // Set desired refresh rate
        //XRDevice.fovZoomFactor = 1.0f; // sometimes helps trigger XR init
        //OVRManager.display.displayFrequency = 72.0f; // or 90.0f
        //OVRPlugin.systemDisplayFrequency = 75f;
    }
}
