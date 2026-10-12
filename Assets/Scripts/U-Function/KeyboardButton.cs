using easyInputs;
using UnityEngine;

namespace UFunction
{
	public class KeyboardButton : MonoBehaviour
	{
		public string characterString;

		public BEAMComputer computer;

        public bool testClick;

		public float pressTime;

        public float Amplitude;

        private void Start()
		{
			pressTime = 0.2f;
			//computer = BEAMComputer.instance;
		}

		public void Update()
		{
			if (testClick)
			{
                computer.PressButton(this);
				testClick = false;
			}
		}

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log("Collision detected with: " + collider.name, collider);

            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
            {
                Debug.Log("Button pressed.");

                // Check if the computer reference is valid before using it
                if (computer != null)
                {
                    computer.PressButton(this);
                }
                else
                {
                    Debug.LogWarning("Computer reference is null. Ensure it is properly assigned.");
                }

                GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();

                if (component != null)
                {
                    PlayerBase.i.GorillaVibration.StartVibration(component.isLeftHand, GorillaVibration.taggedHapticStrength / 2f, GorillaVibration.taggedHapticDuration);
                }
            }
        }
    }
}
