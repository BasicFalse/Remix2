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
                PlayerBase.i.GorillaVibration.StartVibration(component.isLeftHand, GorillaVibration.tapHapticStrength / 2f, GorillaVibration.tapHapticDuration);
            }
        }
    }
}
