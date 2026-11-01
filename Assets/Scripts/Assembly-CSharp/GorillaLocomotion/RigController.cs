using System;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
#if UNITY_EDITOR
using Steamworks;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GorillaRigging{
public class RigController : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    public enum RigType
    {
        Offline, Online, OurOnline, Extern
    }

    public RigType RigStatus;

#pragma warning disable UDR0001 // Domain Reload Analyzer
        public static RigController Instance {get; private set;}
#pragma warning restore UDR0001 // Domain Reload Analyzer
        public RigOffsets Offsets;

    public bool initializedRig;
    public bool initializedCosmetics;
    public bool inTryOnRoom;

    public bool muted;
    public bool mutedTemp;

    public bool isFlashlightOn;

    public bool isGettingJumped;
    public bool isNearMonster;

    private bool LinkWeightsSet = false;
    private bool VRMapWeightsSet = false;

    public float timeSpawned;

    [Range(0,1)] public float sparkleDeathProgress;

    /// <summary>
    /// How interesting the player is, monsters usually prefer higher interest.
    /// Directly tied to player movement.
    /// Also could be used as an arbitrary value for fuck all.
    /// </summary>
    public float Interest { get; private set; }
    protected const float maxInterest = 500f;

    protected float onlineBodyOffest;
    protected const float lerpValueFingers = 0.34f;
    protected const float lerpValueMain = 1f;
    protected const float networkInterpTime = 0.1f;

    protected float ratio;

    public float[] color;

    public int handSync;


    protected RigLink headLink = new();
    protected RigLink bodyLink = new();
    protected RigLink handRight = new();
    protected RigLink handLeft = new();

    protected VRMapThumb leftThumb = new();
    protected VRMapIndex leftIndex = new();
    protected VRMapMiddle leftMiddle = new();
    protected VRMapThumb rightThumb = new();
    protected VRMapIndex rightIndex = new();
    protected VRMapMiddle rightMiddle = new();

    public event Action<Color> OnColorChanged;

    public Text playerTextName;
    public Material playerMaterial;
    public SkinnedMeshRenderer thisGuysSkin;

    public Transform OfflineCamera;
    public Transform OfflineBody;
    public Transform MidwayHandL;
    public Transform MidwayHandR;
    public Transform bodyDownVector;
    public Transform HeadFollow;
    public Transform BodyLocal;

    public CosmeticContainer cosmetics;

    public GameObject flashlight;

    protected CosmeticsController.CosmeticSet wornCosmetics;
    protected CosmeticsController.CosmeticSet wornTryOnCosmetics;

    public AudioSource leftHandAudioSource;
    public AudioSource rightHandAudioSource;

    private void Awake()
    {
        if(RigStatus == RigType.Offline){
            if (Instance == null)
            {
                Instance = this;
            }
            else { if (Instance != this) { Destroy(this); } }
        }
    }

    private void InitializeRig()
    {
        if (RigStatus == RigType.Extern)
        {
            
        }

        color = new float[3];

        Interest = Mathf.Clamp(Interest, 0f, maxInterest);

        if (RigStatus == RigType.Offline)
        {
            CosmeticsController.instance.RefreshRigCosmetics();
            GorillaComputer.instance.RefreshRigColor();

            OfflineCamera = PlayerBase.@i.transforms.Camera.transform;
            OfflineBody = PlayerBase.@i.transforms.Body.transform;

            // Initialize links from current transforms
            headLink?.InitializeFromCurrent();
            bodyLink?.InitializeFromCurrent();
            handLeft?.InitializeFromCurrent();
            handRight?.InitializeFromCurrent();


        }

        if (RigStatus == RigType.Online)
        {
            if (photonView.IsMine)
            {
                RigStatus = RigType.OurOnline;
                thisGuysSkin.enabled = false;
                playerTextName.enabled = false;
                OfflineCamera = PlayerBase.@i.transforms.Camera.transform;
                OfflineBody = PlayerBase.@i.transforms.Body.transform;
                MidwayHandL = PlayerBase.@i.transforms.LeftController;
                MidwayHandR = PlayerBase.@i.transforms.RightController;
                PlayerBase.@i.GorillaVibration.myVRRig = this;
                if ((bool)GetComponent<VoiceConnection>() && (bool)GetComponent<Recorder>())
			    {
			    	GetComponent<VoiceConnection>().InitRecorder(GetComponent<Recorder>());
			    }
			    float red = PlayerPrefs.GetFloat("redValue");
			    float green = PlayerPrefs.GetFloat("greenValue");
			    float blue = PlayerPrefs.GetFloat("blueValue");
                SyncMaterialLocal(red/9, green/9, blue/9);
                PhotonView.Get(this).RPC("SyncMaterial",PhotonView.Get(this).Owner, red/9, green/9, blue/9);
            }
            else
            {
                PhotonView.Get(this).RPC("RequestMaterialColor", PhotonView.Get(this).Owner, PhotonNetwork.LocalPlayer, false);
            }

            if (Offsets)
            {
                //headLink.PositionOffset = Offsets.HeadOffset;
                //headLink.LocalPosOffset = Offsets.HeadLocalOffset;s
            }

            //if (FindFirstObjectByType<PhotonRoomManagerMain>() == null)
            //{
            //}
            //else
            //{
                photonView.RPC("RequestCosmetics", photonView.Owner);
            //}


            // Initialize links
            headLink?.InitializeFromCurrent();
            bodyLink?.InitializeFromCurrent();
            handLeft?.InitializeFromCurrent();
            handRight?.InitializeFromCurrent();


        }
        initializedRig = true;
    }

    private void Update()
    {
            if (!initializedRig)
            {
                InitializeRig();
            }

        if (RigStatus == RigType.Extern) return; // nah

        if (!LinkWeightsSet || !VRMapWeightsSet) return;

        if (RigStatus == RigType.Offline)
        {
            Vector3 gravityDown = -bodyDownVector.up;

            Vector3 camForward = Vector3.ProjectOnPlane(OfflineCamera.forward, gravityDown).normalized;

            Quaternion targetRot = Quaternion.LookRotation(camForward, -gravityDown);

            bodyLink.Rotation = Quaternion.Slerp(
                bodyLink.Rotation,
                targetRot,
                Time.deltaTime * 8f
            );

            Vector3 localOffset = OfflineBody.InverseTransformPoint(OfflineCamera.position);
            Vector3 worldOffsetUndo = OfflineBody.TransformVector(localOffset);
            Vector3 finalPosition = OfflineBody.position + worldOffsetUndo;

            bodyLink.Position = finalPosition;
            //bodyLink.Position = LocomotionMain.Instance.bodyCollider.transform.position;

            bodyLink.ApplySmooth(lerpValueMain);

            BodyLocal.localPosition = new Vector3(BodyLocal.localPosition.x, BodyLocal.localPosition.y, PitchOffsetFromGravity(PlayerBase.@i.transforms.Camera, bodyDownVector) - 0.1f);

            headLink.SetPosAndRot(OfflineCamera.position, OfflineCamera.rotation);
            headLink.ApplySmooth(lerpValueMain);

            rightIndex.MapMyFinger(lerpValueFingers);
            rightMiddle.MapMyFinger(lerpValueFingers);
            rightThumb.MapMyFinger(lerpValueFingers);
            leftIndex.MapMyFinger(lerpValueFingers);
            leftMiddle.MapMyFinger(lerpValueFingers);
            leftThumb.MapMyFinger(lerpValueFingers);

            //color[0] = thisGuysSkin.materials[0].color.r;
            //color[1] = thisGuysSkin.materials[0].color.g;
            //color[2] = thisGuysSkin.materials[0].color.b;

            isFlashlightOn = PlayerBase.@i.bools.isFlashlightOn;

            Interest -= Interest*0.0002f;

            CalculateInterest();
            Interest = Mathf.Clamp(Interest, 0f, maxInterest);


            if (!initializedCosmetics)
            {
                initializedCosmetics = true;
                SetCosmeticsActive();
            }

            return; // ngl kinda useless
        }
    }

    private void LateUpdate()
    {
        // online stuff only haha
        if (RigStatus == RigType.Offline || RigStatus == RigType.Extern) return;

        if (!LinkWeightsSet || !VRMapWeightsSet) return;

        if (LiveEventManager.instance.isPlaying && GorillaComputer.instance.muteAllDuringEvents)
        {
            mutedTemp = true;
        }
        else
        {
            mutedTemp = false;
        }

        if (photonView == null || photonView.Owner == null || (photonView.Owner != null && PhotonNetwork.CurrentRoom.Players.TryGetValue(photonView.Owner.ActorNumber, out Player tempPlayer) && tempPlayer == null))
		{
			GorillaParent.instance.vrrigs.Remove(this);
			if (photonView != null)
			{
				GorillaParent.instance.vrrigDict.Remove(photonView.Owner);
			}
			Destroy(gameObject);
		}

        if (RigStatus == RigType.OurOnline)
        {
            ratio = PlayerBase.@i.getSizeRatio;
            transform.localScale = new Vector3(ratio, ratio, ratio);
            // Keep existing behavior for OurOnline (local authoritative rig)
            bodyLink.Position = Instance.bodyLink.Position;
            bodyLink.Rotation = Instance.bodyLink.Rotation;

            // This copies the controlling transforms directly (we keep your original snap behavior for OurOnline)
            bodyLink.SetPosAndRot(Instance.bodyLink.Controlling.position, Instance.bodyLink.Controlling.rotation);
            bodyLink.ApplySmooth(1);

            BodyLocal.localPosition = new Vector3(BodyLocal.localPosition.x, BodyLocal.localPosition.y, PitchOffsetFromGravity(PlayerBase.@i.transforms.Camera, bodyDownVector) - 0.1f);
            onlineBodyOffest = BodyLocal.localPosition.z;

            headLink.SetPosAndRot(OfflineCamera.position, OfflineCamera.rotation);
            headLink.ApplySmooth(1);

            handRight.SetPosAndRot(Instance.handRight.Controlling.position, Instance.handRight.Controlling.rotation);
            handRight.ApplySmooth(1);
            handLeft.SetPosAndRot(Instance.handLeft.Controlling.position, Instance.handLeft.Controlling.rotation);
            handLeft.ApplySmooth(1);

            rightIndex.MapMyFinger(lerpValueFingers);
            rightMiddle.MapMyFinger(lerpValueFingers);
            rightThumb.MapMyFinger(lerpValueFingers);
            leftIndex.MapMyFinger(lerpValueFingers);
            leftMiddle.MapMyFinger(lerpValueFingers);
            leftThumb.MapMyFinger(lerpValueFingers);

            color = Instance.color;

            isFlashlightOn = PlayerBase.@i.bools.isFlashlightOn;
            flashlight.SetActive(false);
            sparkleDeathProgress = Instance.sparkleDeathProgress;
            Interest = Instance.Interest;

            return; // samesamebutdifferent
        }

        if (RigStatus == RigType.Online)
        {
            transform.localScale = new Vector3(ratio, ratio, ratio);

            // smoothing shit
            bodyLink?.ApplyNetworkSmooth(Time.deltaTime);
            headLink?.ApplyNetworkSmooth(Time.deltaTime);
            handRight?.ApplyNetworkSmooth(Time.deltaTime);
            handLeft?.ApplyNetworkSmooth(Time.deltaTime);

            // maintain local offset
            BodyLocal.localPosition = new Vector3(BodyLocal.localPosition.x, BodyLocal.localPosition.y, onlineBodyOffest);

            rightIndex.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[5]) / 10f, lerpValueFingers);
            rightMiddle.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[4]) / 10f, lerpValueFingers);
            rightThumb.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[3]) / 10f, lerpValueFingers);
            leftIndex.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[2]) / 10f, lerpValueFingers);
            leftMiddle.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[1]) / 10f, lerpValueFingers);
            leftThumb.MapOtherFinger((float)char.GetNumericValue(handSync.ToString().PadLeft(6)[0]) / 10f, lerpValueFingers);

            if (!initializedCosmetics)
            {
                initializedCosmetics = true;
                SetCosmeticsActive();
            }
            flashlight.SetActive(isFlashlightOn);
            GetComponent<PhotonVoiceView>().SpeakerInUse.enabled = !(muted || mutedTemp);
        }
    }

    public float PitchOffsetFromGravity(Transform cam, Transform bodyDownVector)
    {
        // Gravity up/down
        Vector3 gravityDown = -bodyDownVector.up;
        Vector3 gravityUp = bodyDownVector.up;

        // Camera forward
        Vector3 forward = cam.forward;

        // Project forward onto plane orthogonal to gravityDown (removes up/down component)
        Vector3 projected = Vector3.ProjectOnPlane(forward, gravityDown);

        // If projected is nearly zero, camera is looking nearly exactly up or down
        float absAngle;
        const float eps = 1e-6f;
        if (projected.sqrMagnitude < eps)
        {
            absAngle = 90f; // straight up or straight down
        }
        else
        {
            // Angle between the forward and its projection = pitch magnitude (0..90)
            absAngle = Vector3.Angle(projected, forward);
        }

        // Sign: positive if forward has an upward component relative to gravityUp (looking up),
        // negative otherwise (looking down). If exactly level, dot =~ 0 -> sign doesn't matter because absAngle==0.
        float sign = Vector3.Dot(forward, gravityUp) >= 0f ? 1f : -1f;

        // Normalize 0..90 -> 0..1 and scale to 0.1, then apply sign
        float normalized = Mathf.InverseLerp(0f, 90f, absAngle);
        return sign * normalized * 0.1f;
    }

    [PunRPC]
    public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume)
    {
        if (soundIndex > -1 && soundIndex < PlayerBase.@i.resources.materialData.materialData.Count)
        {
            if (isLeftHand)
            {
                leftHandAudioSource.volume = tapVolume;
                leftHandAudioSource.clip = PlayerBase.@i.resources.materialData.materialData[soundIndex].overrideAudio ? PlayerBase.@i.resources.materialData.materialData[soundIndex].audio : PlayerBase.@i.resources.materialData.materialData[0].audio;
                leftHandAudioSource.PlayOneShot(leftHandAudioSource.clip);
            }
            else
            {
                rightHandAudioSource.volume = tapVolume;
                rightHandAudioSource.clip = PlayerBase.@i.resources.materialData.materialData[soundIndex].overrideAudio ? PlayerBase.@i.resources.materialData.materialData[soundIndex].audio : PlayerBase.@i.resources.materialData.materialData[0].audio;
                rightHandAudioSource.PlayOneShot(rightHandAudioSource.clip);
            }
        }
    }

    [PunRPC]
    public void RequestMaterialColor(Photon.Realtime.Player askingPlayer, bool noneBool)
    {
        // Broadcast the owner's color to everyone and buffer it for late joiners.
        // Using RpcTarget.AllBuffered ensures late joiners receive the last color state.
        Debug.Log($"Request color as {playerMaterial.color.r} + {playerMaterial.color.g} + {playerMaterial.color.b}");
        PhotonView.Get(this).RPC(
            "SyncMaterial",
            askingPlayer,
            playerMaterial.color.r,
            playerMaterial.color.g,
            playerMaterial.color.b
        );
    }

    [PunRPC]
    public void SyncMaterial(float red, float green, float blue, PhotonMessageInfo info)
    {
        Debug.Log($"Got color as {red} + {green} + {blue}");
        if (info.Sender == base.photonView.Owner)
        {
            red = Mathf.Clamp(red, 0f, 1f);
            green = Mathf.Clamp(green, 0f, 1f);
            blue = Mathf.Clamp(blue, 0f, 1f);
            SyncMaterialLocal(red, green, blue);
        }
        Color newColor = new Color(
            Mathf.Clamp01(red),
            Mathf.Clamp01(green),
            Mathf.Clamp01(blue),
            1f
        );
        //thisGuysSkin.material.color = newColor;
        // Notify scoreboard instantly
        OnColorChanged?.Invoke(newColor);

    }

    public void SyncMaterialLocal(float red, float green, float blue)
    {

        playerMaterial = thisGuysSkin.material;

        // Clamp and build color
        Color newColor = new(
            Mathf.Clamp01(red),
            Mathf.Clamp01(green),
            Mathf.Clamp01(blue),
            1f
        );

        thisGuysSkin.material.color = newColor;
        // Notify scoreboard instantly
        OnColorChanged?.Invoke(newColor);

        // Apply to both common shader property names
        playerMaterial.color = newColor;
        if (playerMaterial.HasProperty("_BaseColor"))
            playerMaterial.SetColor("_BaseColor", newColor);

        // Apply directly to renderer’s current instance to ensure visibility
        thisGuysSkin.material.color = newColor;
        if (thisGuysSkin.material.HasProperty("_BaseColor"))
            thisGuysSkin.material.SetColor("_BaseColor", newColor);

        try
        {
            OnColorChanged?.Invoke(newColor);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[RigController] OnColorChanged handler threw: {ex}");
        }

        // Update UI text
        if (RigStatus != RigType.Offline && RigStatus != RigType.Extern)
            playerTextName.text = NormalizeName(true, photonView.Owner.NickName);
        else
            playerTextName.text = PlayerPrefs.GetString("playerName", "gorilla");

        Debug.Log($"[{name}] Applied color: {newColor}");
    }

    public string NormalizeName(bool doIt, string text)
    {
        if (doIt)
        {
            text = new string(Array.FindAll(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
            if (text.Length > 12)
            {
                text = text.Substring(0, 11);
            }
            text = text.ToUpper();
        }
        return text;
    }

    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (RigStatus != RigType.Extern)
        {
            timeSpawned = Time.time;
            base.transform.parent = GorillaParent.instance.GetComponent<GorillaParent>().vrrigParent.transform;
            GorillaParent.instance.vrrigs.Add(this);
            if (GorillaParent.instance.vrrigDict.ContainsKey(base.photonView.Owner))
            {
                GorillaParent.instance.vrrigDict[base.photonView.Owner] = this;
            }
            else
            {
                GorillaParent.instance.vrrigDict.Add(base.photonView.Owner, this);
            }
            if (PhotonRoomManagerMain.instance != null && PhotonRoomManagerMain.instance.GetComponent<PhotonView>().IsMine)
            {
                object value;
                bool didTutorial = base.photonView.Owner.CustomProperties.TryGetValue("didTutorial", out value) && !(bool)value;
                Debug.Log("guy who just joined didnt do the tutorial already: " + didTutorial);
                PhotonRoomManagerMain.instance.NewVRRig(base.photonView.Owner, base.photonView.ViewID, didTutorial);
            }
            if(GorillaParent.instance != null && !GorillaParent.instance.vrrigs.Contains(this))
                {
                    GorillaParent.instance.vrrigs.Add(this);
                }
            Debug.Log(base.photonView.Owner.UserId, this);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (RigStatus != RigType.Extern && RigStatus != RigType.Offline)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(bodyLink.Controlling != null ? bodyLink.Controlling.position : bodyLink.Position);
                stream.SendNext(bodyLink.Controlling != null ? bodyLink.Controlling.rotation : bodyLink.Rotation);

                stream.SendNext(headLink.Controlling != null ? headLink.Controlling.position : headLink.Position);
                stream.SendNext(headLink.Controlling != null ? headLink.Controlling.rotation : headLink.Rotation);

                stream.SendNext(Instance.handRight.Controlling.position);
                stream.SendNext(Instance.handRight.Controlling.rotation);
                stream.SendNext(Instance.handLeft.Controlling.position);
                stream.SendNext(Instance.handLeft.Controlling.rotation);

                stream.SendNext(bodyDownVector.rotation);

                stream.SendNext(onlineBodyOffest);
                stream.SendNext(ReturnHandPosition());
                stream.SendNext(color);

                stream.SendNext(Instance.isFlashlightOn);
                stream.SendNext(Instance.isGettingJumped);
                stream.SendNext(Instance.isNearMonster);
                stream.SendNext(Instance.sparkleDeathProgress);
                stream.SendNext(Instance.Interest);
                stream.SendNext(PlayerBase.@i.getSizeRatio);
                //stream.SendNext(leftHandItem);
                //stream.SendNext(rightHandItem);
            }
            else
            {
                // Receive values and set network targets for smoothing.
                Vector3 rBodyPos = (Vector3)stream.ReceiveNext();
                Quaternion rBodyRot = (Quaternion)stream.ReceiveNext();
                bodyLink.SetNetworkTarget(rBodyPos, rBodyRot, networkInterpTime);

                Vector3 rHeadPos = (Vector3)stream.ReceiveNext();
                Quaternion rHeadRot = (Quaternion)stream.ReceiveNext();
                headLink.SetNetworkTarget(rHeadPos, rHeadRot, networkInterpTime);

                Vector3 rHandRPos = (Vector3)stream.ReceiveNext();
                Quaternion rHandRRot = (Quaternion)stream.ReceiveNext();
                handRight.SetNetworkTarget(rHandRPos, rHandRRot, networkInterpTime);

                Vector3 rHandLPos = (Vector3)stream.ReceiveNext();
                Quaternion rHandLRot = (Quaternion)stream.ReceiveNext();
                handLeft.SetNetworkTarget(rHandLPos, rHandLRot, networkInterpTime);

                bodyDownVector.rotation = (Quaternion)stream.ReceiveNext();

                onlineBodyOffest = (float)stream.ReceiveNext();
                handSync = (int)stream.ReceiveNext();
                color = (float[])stream.ReceiveNext();

                isFlashlightOn = (bool)stream.ReceiveNext();
                isGettingJumped = (bool)stream.ReceiveNext();
                isNearMonster = (bool)stream.ReceiveNext();
                sparkleDeathProgress = (float)stream.ReceiveNext();
                Interest = (float)stream.ReceiveNext();
                ratio = (float)stream.ReceiveNext();
                //leftHandItem          = (int)stream.ReceiveNext();
                //rightHandItem         = (int)stream.ReceiveNext();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (photonView.Owner == otherPlayer) { 
            InitiateLocalSelfDestructSequence();
         }
    }

    public void GetUserCosmeticsAllowed()
    {
        if (!(CosmeticsController.instance != null)) return;
        initializedCosmetics = true;
        SetCosmeticsActive();
    }

    public void SetCosmeticsActive()
    {
        if (CosmeticsController.instance == null) { return; }

        if (!inTryOnRoom)
        {
            foreach (GameObject go in cosmetics.cosmeticslist)
            {
                    go.SetActive(AnyCosmSetMatch(wornCosmetics, go.name));
            }
        }
        else
        {
            foreach (GameObject go in cosmetics.cosmeticslist)
            {
                go.SetActive(AnyCosmSetMatch(wornTryOnCosmetics, go.name));
            }
        }
    }

    public bool AnyCosmSetMatch(CosmeticsController.CosmeticSet setToMatch, string nameToMatch)
    {
        if (

        setToMatch.badge.displayName == nameToMatch ||
        setToMatch.face.displayName == nameToMatch ||
        setToMatch.hat.displayName == nameToMatch ||
        setToMatch.chest.displayName == nameToMatch ||
        setToMatch.holdable.displayName == nameToMatch)
        {
            return true;
        }
        else { return false; }
    }

    public int ReturnHandPosition()
    {
        return Mathf.FloorToInt(rightIndex.calcT * 9.99f) + Mathf.FloorToInt(rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(leftThumb.calcT * 9.99f) * 100000;
    }

    [PunRPC]
    public void RequestCosmetics(PhotonMessageInfo info)
    {
        if (base.photonView.IsMine && CosmeticsController.instance != null)
        {
            CosmeticsController.CosmeticSet cosmSetWorn = CosmeticsController.instance.currentWornSet;
            CosmeticsController.CosmeticSet cosmSetTryOn = CosmeticsController.instance.tryOnSet;
            base.photonView.RPC("UpdateCosmeticsWithTryon", info.Sender, cosmSetWorn.hat.displayName, cosmSetWorn.badge.displayName, cosmSetWorn.face.displayName, cosmSetWorn.chest.displayName, cosmSetWorn.holdable.displayName,
                                                                         cosmSetTryOn.hat.displayName, cosmSetTryOn.badge.displayName, cosmSetTryOn.face.displayName, cosmSetTryOn.chest.displayName, cosmSetTryOn.holdable.displayName);
        }
    }
    public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet cosmSet)
    {
        wornCosmetics = cosmSet;
        if (initializedCosmetics)
        {
            SetCosmeticsActive();
        }
    }

    public void LocalUpdateCosmeticsWithTryon(string newHat, string newBadge, string newFace, string newChest, string newHoldable, string newTHat, string newTBadge, string newTFace, string newTChest, string newTHoldable)
    {
        CosmeticsController.CosmeticSet cosmSetWorn = new CosmeticsController.CosmeticSet();
        cosmSetWorn.hat.displayName = newHat;
        cosmSetWorn.badge.displayName = newBadge;
        cosmSetWorn.face.displayName = newFace;
        cosmSetWorn.chest.displayName = newChest;
        cosmSetWorn.holdable.displayName = newHoldable;
        CosmeticsController.CosmeticSet cosmSetTryOn = new CosmeticsController.CosmeticSet();
        cosmSetTryOn.hat.displayName = newTHat;
        cosmSetTryOn.badge.displayName = newTBadge;
        cosmSetTryOn.face.displayName = newTFace;
        cosmSetTryOn.chest.displayName = newTChest;
        cosmSetTryOn.holdable.displayName = newTHoldable;

        wornCosmetics = cosmSetWorn;
        wornTryOnCosmetics = cosmSetTryOn;
        if (initializedCosmetics)
        {
            SetCosmeticsActive();
        }
    }

    public void OnDestroy()
    {
        //GorillaParent.instance.Rigs.Remove(this);
    }

    [PunRPC]
    public void UpdateCosmetics(string newHat, string newBadge, string newFace, string newChest, string newHoldable, PhotonMessageInfo info)
    {
        if (info.Sender == base.photonView.Owner)
        {
            CosmeticsController.CosmeticSet tmp = new CosmeticsController.CosmeticSet();
            tmp.hat.displayName = newHat;
            tmp.badge.displayName = newBadge;
            tmp.face.displayName = newFace;
            tmp.chest.displayName = newChest;
            tmp.holdable.displayName = newHoldable;
            LocalUpdateCosmetics(tmp);
        }
    }

    [PunRPC]
    public void UpdateCosmeticsWithTryon(string newHat, string newBadge, string newFace, string newChest, string newHoldable, string newTHat, string newTBadge, string newTFace, string newTChest, string newTHoldable, PhotonMessageInfo info)
    {
        if (info.Sender == base.photonView.Owner)
        {

            LocalUpdateCosmeticsWithTryon(newHat, newBadge, newFace, newChest, newHoldable, newTHat, newTBadge, newTFace, newTChest, newTHoldable);
        }
    }
    public void ApplyVRMapWeights(VRMapThumb thumbLeft, VRMapIndex indexLeft, VRMapMiddle middleLeft, VRMapThumb thumbRight, VRMapIndex indexRight, VRMapMiddle middleRight)
    {
        if (!VRMapWeightsSet)
        {
            leftThumb = new(thumbLeft);
            leftIndex = new(indexLeft);
            leftMiddle = new(middleLeft);
            rightThumb = new(thumbRight);
            rightIndex = new(indexRight);
            rightMiddle = new(middleRight);

            VRMapWeightsSet = true;
        }
    }
    public void ApplyLinkWeights(Transform head, Transform body, Transform handR, Transform handL)
    {
        if(!LinkWeightsSet)
        {
            headLink    = new(head);
            bodyLink    = new(body);
            handRight   = new(handR);
            handLeft    = new(handL);
            LinkWeightsSet = true;
        }
    }
    public void InitiateLocalSelfDestructSequence()
    {
        Debug.Log("Debating weather the rig should get deleted", this);
        if(RigStatus == RigType.Offline || RigStatus == RigType.Extern) {  Debug.Log("Rig not removed, reason = rig is either offline or extern", this);  return;}
        if(RigStatus == RigType.OurOnline && !GorillaComputer.instance.isInRoom || RigStatus == RigType.Online && !GorillaComputer.instance.isInRoom)
            {
                Debug.Log("Rig removed, reason = online rig instanced while offline", this);
                if(GorillaParent.instance)
                    if (GorillaParent.instance.vrrigs.Contains(this))
                    {
                        GorillaParent.instance.vrrigs.Remove(this);
                    }
                UnityEngine.Object.Destroy(base.gameObject);
            }
        if(RigStatus == RigType.Online && photonView.Owner == null)
            {
                Debug.Log("Rig removed, reason = online rig has no owner", this);
                if(GorillaParent.instance)
                    if (GorillaParent.instance.vrrigs.Contains(this))
                    {
                        GorillaParent.instance.vrrigs.Remove(this);
                    }
                UnityEngine.Object.Destroy(base.gameObject);
            }
        if(RigStatus == RigType.Online && PhotonRoomManagerMain.instance.playerVRRigDict.TryGetValue(photonView.CreatorActorNr, out _))
            {
                Debug.Log("Rig removed, reason = online rig's player match not found", this);
                if(GorillaParent.instance)
                    if (GorillaParent.instance.vrrigs.Contains(this))
                    {
                        GorillaParent.instance.vrrigs.Remove(this);
                    }
                UnityEngine.Object.Destroy(base.gameObject);
            }
        if(RigStatus == RigType.Online && PhotonNetworkController.instance.currentStateVisible == PhotonNetworkController.ConnectionState.JoiningPublicRoom)
            {
                Debug.Log("Rig not removed, reason = rig is an early instance", this);
                return;
            }
        if(RigStatus == RigType.Online && PhotonNetworkController.instance.currentStateVisible == PhotonNetworkController.ConnectionState.JoiningSpecificRoom)
            {
                Debug.Log("Rig not removed, reason = rig is an early instance", this);
                return;
            }
        //if(RigStatus == RigType.Online && PhotonNetworkController.instance.currentStateVisible == PhotonNetworkController.ConnectionState.InPrivateRoom)
        //if(RigStatus == RigType.Online && PhotonNetworkController.instance.currentStateVisible == PhotonNetworkController.ConnectionState.InPublicRoom)
        
        Debug.LogError("Rig not removed, reason = stalemate reached. cannot prove or disprove existance. investigate conditions.", this);
    }
    public void CalculateInterest()
    {
        Interest +=
            (Math.Abs(headLink.GetCorrectionDistance()) +
            Math.Abs(bodyLink.GetCorrectionDistance()) +
            Math.Abs(handRight.GetCorrectionDistance()) +
            Math.Abs(handLeft.GetCorrectionDistance())) * 8;

        Interest += UnityEngine.Random.Range(-Interest/5, Interest/10);

        

        //interest = Mathf.Clamp(interest, 0f, maxInterest);
    }

}
}