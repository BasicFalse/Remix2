using System;
using GorillaRigging;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMapIndex : VRMap
{
	public InputFeatureUsage inputAxis;

	public float triggerTouch;

	public float triggerValue;

	public Transform fingerBone1;

	public Transform fingerBone2;

	public Transform fingerBone3;

	public float closedAngles;

	public Vector3 closedAngle1;

	public Vector3 closedAngle2;

	public Vector3 closedAngle3;

	public Vector3 startingAngle1;

	public Vector3 startingAngle2;

	public Vector3 startingAngle3;

	private InputDevice myInputDevice;

	public VRMapIndex()
	{
		
	}

	public VRMapIndex(VRMapIndex index)
	{
		vrTargetNode = index.vrTargetNode;
		inputAxis = index.inputAxis;
		fingerBone1 = index.fingerBone1;
		fingerBone2 = index.fingerBone2;
		fingerBone3 = index.fingerBone3;
		closedAngles = index.closedAngles;
		closedAngle1 = index.closedAngle1;
		closedAngle2 = index.closedAngle2;
		closedAngle3 = index.closedAngle3;
		startingAngle1 = index.startingAngle1;
		startingAngle2 = index.startingAngle2;
		startingAngle3 = index.startingAngle3;
	}

	public VRMapIndex(RigSetup.Gorilla.FingerSetup bones)
	{
		fingerBone1 = bones.fingerBone1;
		fingerBone2 = bones.fingerBone2;
		fingerBone3 = bones.fingerBone3;
	}

    [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override void MapMyFinger(float lerpValue)
#pragma warning restore CS0809 // :)
	{
		myInputDevice = InputDevices.GetDeviceAtXRNode(vrTargetNode);
		calcT = 0f;
		myInputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

//#if UNITY_EDITOR
//
//		triggerValue = trigger.GetAxis(hand);
//		
//		calcT = 0.1f * triggerTouch;
//		calcT += 0.9f * triggerValue;
//		LerpFinger(lerpValue);
//		
//#endif

		if (myInputDevice.TryGetFeatureValue(CommonUsages.indexTouch, out triggerTouch))
		{
			calcT = 0.1f * triggerTouch;
			calcT += 0.9f * triggerValue;
			LerpFinger(lerpValue);
		}
		else
		{
			calcT = 1f * triggerValue;
			LerpFinger(lerpValue);
		}
	}

	public override void LerpFinger(float lerpValue)
	{
		fingerBone1.localRotation = Quaternion.Lerp(fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle1), Quaternion.Euler(closedAngle1), calcT), lerpValue);
		fingerBone2.localRotation = Quaternion.Lerp(fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle2), Quaternion.Euler(closedAngle2), calcT), lerpValue);
		fingerBone3.localRotation = Quaternion.Lerp(fingerBone3.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle3), Quaternion.Euler(closedAngle3), calcT), lerpValue);
	}
}
