using System;
using GorillaRigging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[Serializable]
public class VRMapThumb : VRMap
{
	public InputFeatureUsage inputAxis;

	public bool primaryButtonTouch;

	public bool primaryButtonPress;

	public bool secondaryButtonTouch;

	public bool secondaryButtonPress;

	public Transform fingerBone1;

	public Transform fingerBone2;

	public Vector3 closedAngle1;

	public Vector3 closedAngle2;

	public Vector3 startingAngle1;

	public Vector3 startingAngle2;

	public VRMapThumb()
	{
		// strange that this didn't exist prior...
	}

	public VRMapThumb(VRMapThumb thumb)
	{
		vrTargetNode = thumb.vrTargetNode;
		isLeft = thumb.isLeft;
		inputAxis = thumb.inputAxis;
		fingerBone1 = thumb.fingerBone1;
		fingerBone2 = thumb.fingerBone2;
		closedAngle1 = thumb.closedAngle1;
		closedAngle2 = thumb.closedAngle2;
		startingAngle1 = thumb.startingAngle1;
		startingAngle2 = thumb.startingAngle2;
	}

	public VRMapThumb(RigSetup.Gorilla.FingerSetup bones)
	{
		fingerBone1 = bones.fingerBone1;
		fingerBone2 = bones.fingerBone2;
	}

	public override void MapMyFinger(float lerpValue)
	{
		calcT = 0f;

		if(Application.platform == RuntimePlatform.Android){
			UnityEngine.XR.InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(vrTargetNode);
			deviceAtXRNode.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonPress);
			deviceAtXRNode.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryTouch, out primaryButtonTouch);
			deviceAtXRNode.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out secondaryButtonPress);
			deviceAtXRNode.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryTouch, out secondaryButtonTouch);
			if (primaryButtonPress || secondaryButtonPress)
			{
				calcT = 1f;
			}
			else if (primaryButtonTouch || secondaryButtonTouch)
			{
				calcT = 0.1f;
			}
		}
		else
		{
			
		}

		LerpFinger(lerpValue);
	}

	public override void LerpFinger(float lerpValue)
	{
		fingerBone1.localRotation = Quaternion.Lerp(fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle1), Quaternion.Euler(closedAngle1), calcT), lerpValue);
		fingerBone2.localRotation = Quaternion.Lerp(fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle2), Quaternion.Euler(closedAngle2), calcT), lerpValue);
	}
}
