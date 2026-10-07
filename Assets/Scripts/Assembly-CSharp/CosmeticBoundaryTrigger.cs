using UnityEngine;
using GorillaRigging;

public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	public RigController rigRef;

	public void OnTriggerEnter(Collider other)
	{
		rigRef = other.attachedRigidbody.gameObject.GetComponent<RigController>();
		rigRef.inTryOnRoom = true;
		rigRef.SetCosmeticsActive();
	}

	public void OnTriggerExit(Collider other)
	{
		rigRef = other.attachedRigidbody.gameObject.GetComponent<RigController>();
		rigRef.inTryOnRoom = false;
		rigRef.SetCosmeticsActive();
	}
}
