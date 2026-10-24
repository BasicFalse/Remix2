using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaKeyboardButton : GorillaTriggerBox
	{
		public string characterString;

		public GorillaComputer computer;

		public bool testClick;
		private AudioSource AS;

		private void Start()
		{
			computer = GorillaComputer.instance;
			AS = gameObject.AddComponent<AudioSource>();
		}

		public new void Update()
		{
			if (testClick)
			{
				testClick = false;
				computer.PressButton(this);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
				computer.PressButton(this);
				GetComponent<MeshRenderer>().material = computer.pressedMaterial;
				AS.PlayOneShot(computer.buttonClick);
				if (component != null)
				{
					PlayerBase.i.GorillaVibration.StartVibration(component.isLeftHand, GorillaVibration.tapHapticStrength / 2f, GorillaVibration.tapHapticDuration);
				}
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
				GetComponent<MeshRenderer>().material = computer.unpressedMaterial;
				AS.PlayOneShot(computer.buttonClack);
				if (component != null)
				{
					PlayerBase.i.GorillaVibration.StartVibration(component.isLeftHand, GorillaVibration.tapHapticStrength / 2f, GorillaVibration.tapHapticDuration);
				}

			}
		}
	}
}
