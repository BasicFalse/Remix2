using GorillaNetworking;

public class WardrobeItemButton : GorillaPressableButton
{
	public HeadModel controlledModel;

	public CosmeticItem currentCosmeticItem;

	public int cosmeticNum;

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeItemButton(currentCosmeticItem);
	}
}
