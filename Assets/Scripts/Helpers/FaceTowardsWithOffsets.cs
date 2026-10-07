using System.Collections.Generic;
using UnityEngine;

public class FaceTowardsWithOffsets : MonoBehaviour
{
    [System.Serializable]
    public class RotationTarget
    {
        public Transform objectToRotate;
        public Transform target;
        public Vector3 rotationOffset;
    }

    public List<RotationTarget> rotationTargets;

    void Update()
    {
        foreach (var rotationTarget in rotationTargets)
        {
            if (rotationTarget.objectToRotate != null && rotationTarget.target != null)
            {
                // Calculate the direction to the target
                Vector3 directionToTarget = rotationTarget.target.position - rotationTarget.objectToRotate.position;

                // Calculate the rotation needed to look at the target
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

                // Apply the offset
                Quaternion offsetRotation = Quaternion.Euler(rotationTarget.rotationOffset);
                Quaternion finalRotation = lookRotation * offsetRotation;

                // Set the rotation of the object
                rotationTarget.objectToRotate.rotation = finalRotation;
            }
        }
    }
}
