using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeFunctionButton : GorillaPressableButton
{
	public string function;
	public Wardrobe WD;
	public float buttonFadeTime = 0.25f;
	public Image buttonCosmeticImage;

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(function, this);
		StartCoroutine(ButtonColorUpdate());
	}

	public void SetCosmeticImage(CosmeticsController.CosmeticSet currentCosmetics)
	{
		if (buttonCosmeticImage != null)
		{
			switch (function)
			{
				case "hat":
					if (currentCosmetics.hat.itemName == "null")
					{ buttonCosmeticImage.sprite = null; buttonCosmeticImage.enabled = false; }
					else
					{ buttonCosmeticImage.sprite = currentCosmetics.hat.itemPicture; buttonCosmeticImage.enabled = true; }
					break;
				case "face":
					if (currentCosmetics.face.itemName == "null")
					{ buttonCosmeticImage.sprite = null; buttonCosmeticImage.enabled = false; }
					else
					{ buttonCosmeticImage.sprite = currentCosmetics.face.itemPicture; buttonCosmeticImage.enabled = true; }
					break;
				case "badge":
					if (currentCosmetics.badge.itemName == "null")
					{ buttonCosmeticImage.sprite = null; buttonCosmeticImage.enabled = false; }
					else
					{ buttonCosmeticImage.sprite = currentCosmetics.badge.itemPicture; buttonCosmeticImage.enabled = true; }
					break;
				case "chest":
					if (currentCosmetics.chest.itemName == "null")
					{ buttonCosmeticImage.sprite = null; buttonCosmeticImage.enabled = false; }
					else
					{ buttonCosmeticImage.sprite = currentCosmetics.chest.itemPicture; buttonCosmeticImage.enabled = true; }
					break;
				case "holdable":
					if (currentCosmetics.holdable.itemName == "null")
					{ buttonCosmeticImage.sprite = null; buttonCosmeticImage.enabled = false; }
					else
					{ buttonCosmeticImage.sprite = currentCosmetics.holdable.itemPicture; buttonCosmeticImage.enabled = true; }
					break;
				default:
					Debug.LogError("UNDEFINED FUNCTION TYPE, TYPE OF " + function);
					break;
			}
		}
		
	}

	private IEnumerator ButtonColorUpdate()
	{
		buttonRenderer.material = pressedMaterial;
		yield return new WaitForSeconds(buttonFadeTime);
		buttonRenderer.material = unpressedMaterial;
	}
}
