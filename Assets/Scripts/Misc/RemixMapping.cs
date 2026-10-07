using UnityEngine;

public static class RemixMapping{
    public static (Vector3 position, Quaternion rotation) MapRelativeTransform(
        Transform baseObject,    // Object 1 — the reference base
        Transform sourceObject,  // Object 2 — the object being read
        Transform targetObject   // Object 3 — the object being mapped to
    )
    {
        // Get source's local position/rotation relative to the base
        Vector3 localPosition = baseObject.InverseTransformPoint(sourceObject.position);
        Quaternion localRotation = Quaternion.Inverse(baseObject.rotation) * sourceObject.rotation;

        // Apply that same local offset to the target
        Vector3 mappedPosition = targetObject.TransformPoint(localPosition);
        Quaternion mappedRotation = targetObject.rotation * localRotation;

        return (mappedPosition, mappedRotation);
    }
}