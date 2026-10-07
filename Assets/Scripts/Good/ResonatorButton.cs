using UnityEngine;

public class ResonatorButton : GorillaPressableButton
{
    public Resonator Rs;
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        Rs.ButtonPressed();
    }
}
