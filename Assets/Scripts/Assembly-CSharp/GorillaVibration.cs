using System.Collections;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using GorillaRigging;

[RequireComponent(typeof(Rigidbody))]
public class GorillaVibration : MonoBehaviour
{
	public enum StatusEffect
	{
		None,
		Frozen,
		Slowed,
		Dead,
		Infected,
		It
	}

#pragma warning disable UDR0001 // Domain Reload Analyzer
    private static GorillaVibration _instance;
#pragma warning restore UDR0001 // Domain Reload Analyzer

    public MiscGrabs MG;

    private StatusEffect currentStatus;

	public SphereCollider headCollider;

	public CapsuleCollider bodyCollider;

	private Vector3 lastLeftHandPositionForTag;

	private Vector3 lastRightHandPositionForTag;

	private Vector3 lastBodyPositionForTag;

	private Vector3 lastHeadPositionForTag;

	public Transform rightHandTransform;

	public Transform leftHandTransform;

	public float hapticWaitSeconds = 0.05f;

	public float handTapVolume = 0.1f;

	public float tapCoolDown = 0.15f;

	public float lastLeftTap;

	public float lastRightTap;

	public float tapHapticDuration = 0.05f;

	public float tapHapticStrength = 0.5f;

	public float taggedHapticDuration = 0.35f;

	public float taggedHapticStrength = 1f;

	public float statusEndTime;

	private bool leftHandTouching;

	private bool rightHandTouching;

	public RigController myVRRig;

	public RigController offlineVRRig;

	public FlashlightToggle FLT;

	private Vector3 leftRaycastSweep;

	private Vector3 leftHeadRaycastSweep;

	private Vector3 rightRaycastSweep;

	private Vector3 rightHeadRaycastSweep;

	private Vector3 headRaycastSweep;

	private Vector3 bodyRaycastSweep;

	private Vector3 topVector;

	private Vector3 bottomVector;

	private Vector3 bodyVector;

	private int tempInt;

    public static GorillaVibration Instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}

		SpawnManager.SpawnDat sd = MG.SM.CurrentSpawnDat();
		Rigidbody rb2 = GetComponent<Rigidbody>();
		rb2.transform.parent.position = sd.position;
		rb2.transform.position = sd.position;
		rb2.position = sd.position;
		Debug.Log("GORILLA VIBRATION : SETTING PLAYER POSITION ; " + MG.SM.CurrentSpawnDat().position);
		transform.localPosition = Vector3.zero;

		transform.eulerAngles = sd.eulerAngles;

		transform.localPosition = Vector3.zero;

		FLT.forceOff = sd.disableFlashlight;
		

		if (!MG.SM.DidDoTutorial())
		{
			UpdateColor(PlayerPrefs.GetFloat("redValue", Random.value), PlayerPrefs.GetFloat("greenValue", Random.value), PlayerPrefs.GetFloat("blueValue", Random.value));
		}
	}

	private void Start()
	{
		PlayerBase.@i.RPhysical.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
		PlayerBase.@i.RPhysical.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
		bodyVector = new Vector3(0f, bodyCollider.height / 2f - bodyCollider.radius, 0f);
	}

	private void LateUpdate()
	{
		/*
		if (Application.platform != RuntimePlatform.Android)
				{
					if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
					{
						Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
						GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * (1f / 12f)), 10);
						if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
						{
							GorillaLocomotion.Player.Instance.velocityHistorySize--;
						}
						Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize);
						GorillaLocomotion.Player.Instance.InitializeValues();
					}
				}
				if (Application.platform != RuntimePlatform.Android && Mathf.Abs(Time.fixedDeltaTime - 1f / 144f) > 0.0001f)
				{
					Time.fixedDeltaTime = 1f / 144f;
					GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
					if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
					{
						GorillaLocomotion.Player.Instance.velocityHistorySize--;
					}
					Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize);
					GorillaLocomotion.Player.Instance.InitializeValues();
				}
		*/
		leftRaycastSweep = leftHandTransform.position - lastLeftHandPositionForTag;
		leftHeadRaycastSweep = leftHandTransform.position - headCollider.transform.position;
		rightRaycastSweep = rightHandTransform.position - lastRightHandPositionForTag;
		rightHeadRaycastSweep = rightHandTransform.position - headCollider.transform.position;
		headRaycastSweep = headCollider.transform.position - lastHeadPositionForTag;
		bodyRaycastSweep = bodyCollider.transform.position - lastBodyPositionForTag;
		topVector = lastBodyPositionForTag + bodyVector;
		bottomVector = lastBodyPositionForTag - bodyVector;
		if (myVRRig == null && PhotonNetwork.InRoom)
		{
			foreach (RigController vrrig in PhotonRoomManagerMain.instance.rigs)
			{
				if (vrrig != null && vrrig.RigStatus != RigController.RigType.Offline && vrrig.photonView != null && vrrig.photonView.IsMine)
				{
					myVRRig = vrrig;
				}
			}
		}
		if (PlayerBase.@i.RPhysical.IsHandTouching(forLeftHand: true) && !leftHandTouching && Time.time > lastLeftTap + tapCoolDown && !PlayerBase.@i.RPhysical.inOverlay)
		{
			StartVibration(forLeftController: true, tapHapticStrength, tapHapticDuration);
			tempInt = ((PlayerBase.@i.RPhysical.leftHandSurfaceOverride != null) ? PlayerBase.@i.RPhysical.leftHandSurfaceOverride.overrideIndex : PlayerBase.@i.RPhysical.leftHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && myVRRig != null)
			{
				PhotonView.Get(myVRRig).RPC("PlayHandTap", RpcTarget.Others, tempInt, true, handTapVolume);
			}
			offlineVRRig.PlayHandTap(tempInt, isLeftHand: true, handTapVolume);
			lastLeftTap = Time.time;
		}
		if (PlayerBase.@i.RPhysical.IsHandTouching(forLeftHand: false) && !rightHandTouching && Time.time > lastRightTap + tapCoolDown && !PlayerBase.@i.RPhysical.inOverlay)
		{
			StartVibration(forLeftController: false, tapHapticStrength, tapHapticDuration);
			tempInt = ((PlayerBase.@i.RPhysical.rightHandSurfaceOverride != null) ? PlayerBase.@i.RPhysical.rightHandSurfaceOverride.overrideIndex : PlayerBase.@i.RPhysical.rightHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && myVRRig != null)
			{
				PhotonView.Get(myVRRig).RPC("PlayHandTap", RpcTarget.Others, tempInt, false, handTapVolume);
			}
			offlineVRRig.PlayHandTap(tempInt, isLeftHand: false, handTapVolume);
			lastRightTap = Time.time;
		}
		CheckEndStatusEffect();
		leftHandTouching = PlayerBase.@i.RPhysical.IsHandTouching(forLeftHand: true);
		rightHandTouching = PlayerBase.@i.RPhysical.IsHandTouching(forLeftHand: false);
		lastLeftHandPositionForTag = leftHandTransform.position;
		lastRightHandPositionForTag = rightHandTransform.position;
		lastBodyPositionForTag = bodyCollider.transform.position;
		lastHeadPositionForTag = headCollider.transform.position;
	}

	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		StartCoroutine(HapticPulses(forLeftController, amplitude, duration));
	}

	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0u;
		InputDevice device = ((!forLeftController) ? InputDevices.GetDeviceAtXRNode(XRNode.RightHand) : InputDevices.GetDeviceAtXRNode(XRNode.LeftHand));
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, hapticWaitSeconds);
			yield return new WaitForSeconds(hapticWaitSeconds * 0.9f);
		}
	}

	public void ApplyStatusEffect(StatusEffect newStatus, float duration)
	{
		EndStatusEffect(currentStatus);
		currentStatus = newStatus;
		statusEndTime = Time.time + duration;
		switch (newStatus)
		{
			case StatusEffect.Frozen:
				PlayerBase.@i.RPhysical.disableMovement = true;
				break;
			case StatusEffect.None:
			case StatusEffect.Slowed:
				break;
		}
	}

	public void EndStatusEffect(StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
			case StatusEffect.Frozen:
				PlayerBase.@i.RPhysical.disableMovement = false;
				currentStatus = StatusEffect.None;
				break;
			case StatusEffect.Slowed:
				currentStatus = StatusEffect.None;
				break;
			case StatusEffect.None:
				break;
		}
	}

	public void CheckEndStatusEffect()
	{
		if (Time.time > statusEndTime)
		{
			EndStatusEffect(currentStatus);
		}
	}

	public void UpdateColor(float red, float green, float blue)
	{
		Debug.Log("GORILLA VIBRATION : SENDING COLORS TO RIG CONTROLLER AS ; " + red/9 + ", " + green/9 + ", " + blue/9);
		offlineVRRig.SyncMaterialLocal(red/9, green/9, blue/9);
		offlineVRRig.thisGuysSkin.material = offlineVRRig.playerMaterial;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.InRoom && other.gameObject.layer == 15)
		{
			if (other.gameObject.GetComponent<GorillaTriggerBox>())
			{
				other.gameObject.GetComponent<GorillaTriggerBox>().OnBoxTriggered();
			}
		}
		if ((bool)other.GetComponentInChildren<GorillaTriggerBox>())
		{
			other.GetComponentInChildren<GorillaTriggerBox>().OnBoxTriggered();
		}
		else if ((bool)other.GetComponentInParent<GorillaTrigger>())
		{
			other.GetComponentInParent<GorillaTrigger>().OnTriggered();
		}
	}
}
