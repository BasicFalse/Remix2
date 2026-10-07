using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

public class FlashlightToggle : RemixScript
{
    public GameObject flashlight; // Assign your flashlight GameObject in the inspector
    public AudioSource audioSource; // Assign your AudioSource in the inspector
    public AudioClip defaultAudioClip; // Assign a default clip in the inspector if desired
    public bool forceOff;
    public bool toggle;
    public bool isOn;
    public PlayerBase youPlayer;

    public bool isFlashlightOn = false;
    private bool previousButtonState = false;

    public override void StartExt()
    {
        // Find and assign the AudioSource if it hasn't been assigned in the Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("No AudioSource found on " + gameObject.name + ". Please add one or assign it in the Inspector.");
            }
        }

        // Set the default audio clip if no clip is assigned
        if (audioSource != null && audioSource.clip == null)
        {
            if (defaultAudioClip != null)
            {
                audioSource.clip = defaultAudioClip;
            }
            else
            {
                // Load an audio clip from Resources if not set in the inspector
                audioSource.clip = Resources.Load<AudioClip>("Audio/DefaultSound"); // Ensure the clip is in a Resources/Audio folder
                if (audioSource.clip == null)
                {
                    Debug.LogWarning("Default audio clip not found in Resources/Audio. Please assign a clip in the inspector or add one to Resources.");
                }
            }
        }
    }

    void Update()
    {
        youPlayer.bools.isFlashlightOn = isOn;
        flashlight.SetActive(isOn);

        if (toggle)
        {
            toggle = false;
            ToggleFlashlight();
        }

        // Get the current state of the B button on the right hand controller
        bool buttonPressed;
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonPressed) && buttonPressed != previousButtonState)
        {
            if (buttonPressed)
            {
                ToggleFlashlight();
                PlaySound();
            }

            // Update previous button state
            previousButtonState = buttonPressed;
        }

        if (forceOff)
        {
            isFlashlightOn = false;
            flashlight.SetActive(isFlashlightOn);
            isOn = false;
        }
    }

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;
        flashlight.SetActive(isFlashlightOn);
        isOn = isFlashlightOn;
    }

    private void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No audio clip assigned or found. Please ensure an audio clip is set.");
        }
    }
}