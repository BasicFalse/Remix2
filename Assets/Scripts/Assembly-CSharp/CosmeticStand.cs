using GorillaNetworking;
using UnityEngine.UI;

public class CosmeticStand : GorillaPressableButton
{
	public CosmeticItem thisCosmeticItem;

	public string thisCosmeticName;

	public HeadModel thisHeadModel;

	public Text slotPriceText;

	public void Start()
    {
        CosmeticsController.instance.AddCosmeticStand(this);
    }

	public void InitializeCosmetic()
	{
		thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find(x => thisCosmeticName == x.displayName);
		slotPriceText.text = thisCosmeticItem.itemSlot.ToUpper() + " " + "ye idk";
	}

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}
}
