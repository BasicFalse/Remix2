using UnityEngine;

[CreateAssetMenu(fileName = "HitSoundRandomizer", menuName = "GorillaLocomotion/HitSoundRandomizer")]
public class HitOverrideSettings : ScriptableObject
{
    [System.Serializable]
    public class OverrideHitRange
    {
        public int RangeMinimum;
        public int RangeMaximum;
    }

    public OverrideHitRange Crystal;
}
