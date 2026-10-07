using UnityEngine;

public class EffectButton : GorillaPressableButton
{
    public FlashEffect effectType;
    public float effectDuration;
    public bool networked;
    public override void ButtonActivation()
    {
        PlayerLocalStatusKeeper.Instance.RequestEffect(effectType, effectDuration, transform.position, networked);
    }
}
