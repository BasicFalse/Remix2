using GorillaNetworking;

public class RefreshWardrobeButton : GorillaPressableButton
{
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        CosmeticsController.instance.RecheckCosmeticsInventory();
    }
}
