using UnityEngine;
using UnityEngine.UI;

public class GorillaPlayerLineButton : MonoBehaviour
{
    public enum ButtonType
    {
        HateSpeech = 0,
        Cheating = 1,
        Toxicity = 2,
        Mute = 3,
        Report = 4,
        Cancel = 5,
        Frnd = 6, // Friend button type
        Options = 7
    }

    public GorillaPlayerScoreboardLine parentLine;

    public ButtonType buttonType;

    public bool isOn;

    public Material offMaterial;

    public Material onMaterial;

    public string offText;

    public string onText;

    public Text myText;

    public float debounceTime = 0.25f;

    public float touchTime;

    public bool testPress;

    private void Update()
    {
        if (testPress)
        {
            testPress = false;

            // Toggle state for Mute and Frnd button types
            if (buttonType == ButtonType.Mute || buttonType == ButtonType.Frnd || buttonType == ButtonType.Options)
            {
                isOn = !isOn;
            }

            parentLine.PressButton(isOn, buttonType);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!base.enabled || !(touchTime + debounceTime < Time.time))
        {
            return;
        }
        touchTime = Time.time;
        Debug.Log("Collision detected: " + collider, collider);

        if (!(collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
        {
            return;
        }

        GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
        Debug.Log("Button press detected");

		// Toggle state for Mute and Frnd button types
		if (buttonType == ButtonType.Mute || buttonType == ButtonType.Frnd || buttonType == ButtonType.Options)
		{
			isOn = !isOn;
		}

        // Allow pressing for these button types
        if (buttonType == ButtonType.Mute || buttonType == ButtonType.Frnd ||
            buttonType == ButtonType.HateSpeech || buttonType == ButtonType.Cheating ||
            buttonType == ButtonType.Cancel || buttonType == ButtonType.Options || parentLine.canPressNextButton)
        {
            parentLine.PressButton(isOn, buttonType);

            // Trigger haptic feedback
            if (component != null)
            {
                PlayerBase.i.GorillaVibration.StartVibration(
                    component.isLeftHand,
                    GorillaVibration.tapHapticStrength / 2f,
                    GorillaVibration.tapHapticDuration
                );
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset report button state when exiting
        if (buttonType != ButtonType.Mute &&
            buttonType != ButtonType.Frnd &&
            buttonType != ButtonType.Options &&
            other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
        {
            parentLine.canPressNextButton = true;
        }
    }

    public void UpdateColor()
    {
        // Update button appearance for "on" and "off" states
        if (isOn)
        {
            GetComponent<MeshRenderer>().material = onMaterial;
            myText.text = onText;
        }
        else
        {
            GetComponent<MeshRenderer>().material = offMaterial;
            myText.text = offText;
        }
    }
}