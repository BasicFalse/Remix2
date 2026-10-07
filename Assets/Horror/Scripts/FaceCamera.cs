using GorillaLocomotion;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public float turnSpeed = 5f; // Rotation speed

    private Transform playerTransform;
    private Transform mainCameraTransform;

    private void Start()
    {
        playerTransform = PlayerBase.@i.transforms.Camera.transform;

        // Get the main camera's transform
        mainCameraTransform = Camera.main.transform;
        if (mainCameraTransform == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    private void Update()
    {
        if (playerTransform == null || mainCameraTransform == null) return;

        // Calculate direction from object to the camera
        Vector3 directionToCamera = mainCameraTransform.position - transform.position;

        // Create the target rotation to face the camera
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // Smoothly rotate towards the target rotation based on turnSpeed
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}