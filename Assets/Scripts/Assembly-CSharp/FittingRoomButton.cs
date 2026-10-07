using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class FittingRoomButton : GorillaPressableButton
{
	public CosmeticItem currentCosmeticItem;

	public Image currentImage;

	public MeshRenderer button;

	public string noCosmeticText;

	public Text buttonText;

	public override void StartExt()
	{
		currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	public override void UpdateColor()
	{
		if (currentCosmeticItem.itemName == "null")
		{
			button.material = unpressedMaterial;
			buttonText.text = noCosmeticText;
		}
		else if (isOn)
		{
			button.material = pressedMaterial;
			buttonText.text = onText;
		}
		else
		{
			button.material = unpressedMaterial;
			buttonText.text = offText;
		}
	}

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		//CosmeticsController.instance.PressFittingRoomButton(this);
	}
}
