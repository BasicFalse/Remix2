using System;
using GorillaRigging;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMapMiddle : VRMap
{
	public InputFeatureUsage inputAxis;

	public float gripValue;

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

	public VRMapMiddle()
	{
		
	}

	public VRMapMiddle(VRMapMiddle middle)
	{
		vrTargetNode = middle.vrTargetNode;
		inputAxis = middle.inputAxis;
		fingerBone1 = middle.fingerBone1;
		fingerBone2 = middle.fingerBone2;
		fingerBone3 = middle.fingerBone3;
		closedAngles = middle.closedAngles;
		closedAngle1 = middle.closedAngle1;
		closedAngle2 = middle.closedAngle2;
		closedAngle3 = middle.closedAngle3;
		startingAngle1 = middle.startingAngle1;
		startingAngle2 = middle.startingAngle2;
		startingAngle3 = middle.startingAngle3;
	}

	public VRMapMiddle(RigSetup.Gorilla.FingerSetup bones)
	{
		fingerBone1 = bones.fingerBone1;
		fingerBone2 = bones.fingerBone2;
		fingerBone3 = bones.fingerBone3;
	}

	public override void MapMyFinger(float lerpValue)
	{
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(vrTargetNode);
		calcT = 0f;
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.grip, out gripValue);

//#if UNITY_EDITOR || !UNITY_ANDROID
//
//		gripValue = trigger.GetAxis(hand);
//
//#endif

		calcT = 1f * gripValue;
		LerpFinger(lerpValue);
	}

	public override void LerpFinger(float lerpValue)
	{
		fingerBone1.localRotation = Quaternion.Lerp(fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle1), Quaternion.Euler(closedAngle1), calcT), lerpValue);
		fingerBone2.localRotation = Quaternion.Lerp(fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle2), Quaternion.Euler(closedAngle2), calcT), lerpValue);
		fingerBone3.localRotation = Quaternion.Lerp(fingerBone3.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle3), Quaternion.Euler(closedAngle3), calcT), lerpValue);
	}
}
