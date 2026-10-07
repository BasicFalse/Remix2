using System.Collections;
using UnityEngine;

public class VendingMachineButton : GorillaPressableButton
{
	public VendingMachine.SodaType soda;
	public AudioClip buttonClick;
	public AudioSource buttonSound;
	public VendingMachine VendingMachine;
	public float buttonFadeTime = 0.25f;

	public override void ButtonActivation()
	{
		if (soda != VendingMachine.SodaType.Nothing)
		{
			buttonSound.PlayOneShot(buttonClick);
		}
		VendingMachine.currentSoda = soda;
		base.ButtonActivation();
		StartCoroutine(ButtonColorUpdate());
	}

	private IEnumerator ButtonColorUpdate()
	{
		buttonRenderer.material = pressedMaterial;
		yield return new WaitForSeconds(buttonFadeTime);
		buttonRenderer.material = unpressedMaterial;
	}
}