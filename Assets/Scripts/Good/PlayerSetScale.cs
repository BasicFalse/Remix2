using GorillaLocomotion;
using UnityEngine;

public class PlayerSetScale : MonoBehaviour
{
    public float TargetScale = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if(other == PlayerBase.@i.colliders.bodyCollider)
        {
            PlayerBase.@i.transforms.PlayerRoot.localScale = new Vector3(TargetScale, TargetScale, TargetScale);
        }
    }
}
