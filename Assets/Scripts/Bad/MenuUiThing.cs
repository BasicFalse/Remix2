using UnityEngine;
using UnityEngine.XR;

public class MenuUiThing : MonoBehaviour
{
    public bool didPress;
    public bool menuOpen;
    public float slideSpeed = 5f;
    public Transform RotPiv;

    private InputDevice leftHand;

    void Update()
    {
        if (!leftHand.isValid)
            leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // Detect hamburger/menu button press
        if (leftHand.TryGetFeatureValue(CommonUsages.menuButton, out bool pressed) && pressed)
        {
            if (!didPress)
            {
                didPress = true;
                menuOpen = !menuOpen;
            }
        }
        else
        {
            didPress = false;
        }

        float targetY = menuOpen ? 0f : 180f;

        RotPiv.localRotation = Quaternion.Euler(
            RotPiv.eulerAngles.x,
            targetY,
            RotPiv.eulerAngles.z
        );
    }
}
