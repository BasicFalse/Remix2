using UnityEngine;

[CreateAssetMenu(fileName = "RigOffsets", menuName = "Rig/RigOffsets")]
public class RigOffsets : ScriptableObject
{
    public Vector3 HeadOffset;
    public Vector3 HeadLocalOffset;
    public Vector3 BodyOffset;
    public Vector3 BodyLocalOffset;
}
