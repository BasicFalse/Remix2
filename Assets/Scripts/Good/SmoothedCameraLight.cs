using UnityEngine;

public class SmoothedCameraLight : MonoBehaviour
{
    public float smoothSpeed = 5f;

    public Transform cameraTransform;

    private Quaternion _smoothedRotation;


    private void Awake()
    {
        _smoothedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (smoothSpeed > 0)
        {

            _smoothedRotation = Quaternion.Slerp(
                _smoothedRotation,
                cameraTransform.rotation,
                1f - Mathf.Exp(-smoothSpeed * Time.deltaTime)
            );

            transform.rotation = _smoothedRotation;
        }
        else
        {
            transform.rotation = cameraTransform.rotation;
            _smoothedRotation  = cameraTransform.rotation;
        }
    }

    public void SetSmoothing(int value) =>  smoothSpeed = value;
}
