using UnityEngine;

public class SliderVibrator : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
        {
            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            Debug.Log("slidar slid");
            if (component != null)
            {
                GorillaVibration.Instance.StartVibration(component.isLeftHand, GorillaVibration.Instance.tapHapticStrength / 2f, GorillaVibration.Instance.tapHapticDuration);
            }
        }
    }
}
