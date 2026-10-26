using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using GorillaModManager;
using Photon.Pun;
using RemixLogging;
using RemixNetworking;
using UFunction;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace GorillaNetworking
{
    public class GorillaComputer : MonoBehaviourPunCallbacks
    {
        public enum ComputerState{Startup = 0,Color = 1,Name = 2,Turn = 3,Room = 4,Config = 6,Links = 8,Credits = 10,ERRORVIEW = 12}
        
        public enum ConectivityState{Startup, Conneted, Online, Offline, ConnectonError, Banned, PlayfabError}

        [Serializable]
        public class RegMapSegment
        {
            [SerializeField]
            public string MapName;
            public GorillaLevelScreen[] joinScreens;
            public GorillaLevelScreen[] leaveScreens;
        }

        public ComputerState currentState;

        public ConectivityState conectivityState;

        public static volatile GorillaComputer instance;

        public StartupManager StartupManager;

        public GameObject PlayerPrefab;
        public GameObject PlayerObjectCache;

        public long dateNTimeRawer;

        private int turnValue;
        public int colorCursorLine;
        public int roomModSelect;
        public int cnfgPSelect;
        public int cnfgLSelect;
        public int MirrorQualityLevel;
        public int smoothFlashlightMotion;
        public int cnfgSelectedErrorLog;

        public float redValue;
        public float redText;
        public float blueValue;
        public float blueText;
        public float greenValue;
        public float greenText;
        public float updateCooldown = 1f;
        public float LightCullDistance = 0f;
        public float lastUpdateTime;
        public float buttonFadeTime;

        public string currentRoomMod2;
        public string dateNTime;
        public string configText;
        public string currentTextField;
        public string dataText;
        public string funcText;
        public string roomToJoin;
        private string turnType;
        public string pttType;
        public string currentQueue;
        public string SpawnMapJoin;
        public string currentRoomGeoLoco;
        public string voiceChatOn;
        public string currentGameMode;
        public string currentRoomMod;
        public string currentGameMod;
        public string funcTextType;
        public string savedWantedSpawn;

        public string savedName;
        public string currentName;

        public List<string> urls;

        public readonly string[][] cnfgOptions = new string[][] {
        new string[]{
            "SHOW CAMERA OVERLAY: ",
            "SHOW DEVELOPER VISUALS: ",
            "SHOW DEBUG OVERLAY: ",
            "DISPLAY ERROR LOG",
            "SCROLLING ERROR LOG: "
        },
        new string[]{
            "DISABLE PARTICLES: ",
            "FUNCTION SELECT STYLE: ",
            "AUTO MUTE ALL DURING EVENTS: ",
            "POST PROCESSING: ",
            "SHOW NAMETAGS: ",
            "LIGHT CULL DISTANCE: ",
            "MIRROR QUALITY LEVEL: "
        },
        new string[]{
            "OVERRIDE SPAWN LOCATION: ",
            "FLASHLIGHT MOTION: "
        }
        };
        public readonly string[][] cnfgInfo = new string[][] { 
        new string[]{
            
        }, 
        new string[]{
            "DETERMINES WEATHER PARTICLES ARE SHOWN ON COSMETICS.",
            "DETERMINES THE FUNCTION SELECT STYLE.",
            "DETERMINES IF OTHERS ARE MUTED DURING PRESET OR LIVE EVENTS.",
            "DETERMINES IF POST PROCESSING IS APPLIED.",
            "PUTS IDENTIFIERS ABOVE PLAYERS. [PLAYFAB ID & NAMETAG]",
            "DETERMINES HOW FAR AWAY REALTIME LIGHTS ARE CULLED IN UNITY UNITS. 0 IS OFF.",
            "DETERMINES MIRROR QUALITY. 0 IS OFF, 1 IS BASIC, 2 IS FANCY."
        },
        new string[]{
            "THIS DOES NOT WORK AT THE CURRENT MOMENT.",
            "DERERMINES IF MOTION OF YOUR FLASHLIGHT RELATIVE TO YOUR HEAD."
        }
        };

        private string MOTDText = "";

        private string[] exactOneWeek;
        private string[] anywhereOneWeek;
        private string[] anywhereTwoWeek;

        public bool tryGetTimeAgain;
        public bool canOverrideSpawn;
        public bool disableParticles;
        public bool muteAllDuringEvents;
        public bool enablePostProcessing;
        public bool showNametags;
        public bool roomFull;    
        public bool debug;
        public bool isInRoom => PhotonNetwork.InRoom || PhotonNetwork.InLobby;
        public bool isConnectedToMaster;
        public bool isChangingMods;
        public bool canShowFullMSG;
        public bool showCnfgInfo;
        public bool showCnfgErrorDetail;
        public bool errorViewScroll;

        private bool fetchedMOTD = false;

        public AudioClip buttonClick;
        public AudioClip buttonClack;

        public Camera playerCam => PlayerBase.i.resources.playerCam;

        public DateTime dateNTimeRaw;

        public Material unpressedMaterial;

        public Material pressedMaterial;

        [SerializeField]
        public TextAsset exactOneWeekFile;

        public TextAsset anywhereOneWeekFile;

        public TextAsset anywhereTwoWeekFile;

        public GorillaFriendCollider friendJoinCollider;

        public SnapTurnProvider turnProviderSnap;

        public ContinuousTurnProvider turnProviderSmooth;

        public SmoothedCameraLight flashlightSmoothingControl => PlayerBase.i.resources.FlashlightSmoothing;

        public PhotonNetworkController networkController;

        public CosmeticsController cosmeticsController;

        public NameTagManagers nametagManager;

        public LightCuller lightCuller => PlayerBase.i.resources.lightCulling;

        public TimeFetcher TFs;

        public Volume postProcessingVolume;

        public List<MirrorInstance> mirrorInstances;

        public List<TerminalInstance> Terminals;

        public List<Text> screenTexts;

        public List<Text> functionSelectTexts;

        public ModeSelectButton[] modeSelectButtons;

        public List<RegMapSegment> RegisteredMaps;

        public List<LogDisplay> DebugLogs = new();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(base.gameObject);
            }
            Startup.Update();
            Application.logMessageReceived += Log;
        }
        private void Start()
        {
            PlayerExistanceCheck();
            //foreach (GorillaTerminal gt in terminals)
            {
                //gt.Image.gameObject.SetActive(false);
            }
            conectivityState = ConectivityState.Startup;
            currentTextField = "";
            roomToJoin = "";
            redText = 0;
            blueText = 0;
            greenText = 0;
            currentName = "";
            savedName = "";
            StartCoroutine(WebUtilities.GetRemixLinks(resultList =>
            {
                foreach (var text in resultList)
                    urls.Add(text);
            }));

            canOverrideSpawn = StartupManager.CanSpawnHaveOveride();
            UpdateScreen();
            currentState = ComputerState.Startup;
            InitializeColorState();
            InitializeNameState();
            InitializeRoomState();
            InitializeTurnState();
            InitializeStartupState();
            InitializeQueueState();
            InitializeConfigState();
            InitializeMicState();
            InitializeSpawnState();
            InitializeVoiceState();
            InitializeGameMode();
            InitializeGameMod();
        }
        private void Update()
        {
            PlayerExistanceCheck();
            if (isConnectedToMaster && Time.time > lastUpdateTime + updateCooldown)
            {
                lastUpdateTime = Time.time;
            }
            dateNTime = TFs.timr;
            dateNTimeRaw = TFs.timraw;
            dateNTimeRawer = dateNTimeRaw.ToFileTime();
            if (currentName.Contains("go") || savedName.Contains("go"))
            {
                currentState = ComputerState.Name;
            }
            UpdateScreen();
        }

        public void PlayerExistanceCheck()
        {
            if(PlayerBase.i == null)
            {
                if(PlayerObjectCache != null)
                {
                    Destroy(PlayerObjectCache);
                }
                PlayerObjectCache = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                PlayerObjectCache.GetComponent<PlayerInit>().WakeNewPlayer();
            }
            if(PlayerObjectCache == null)
            {
                PlayerObjectCache = PlayerBase.i.transforms.PlayerRoot.gameObject;
            }
        }

        public void PressButton(GorillaKeyboardButton buttonPressed)
        {
            if(buttonPressed.characterString == "up")
            {
                currentState = currentState switch{
                    ComputerState.Startup   => ComputerState.Room,
                    ComputerState.Room      => ComputerState.Credits,
                    ComputerState.Name      => ComputerState.Room,
                    ComputerState.Color     => ComputerState.Name,
                    ComputerState.Turn      => ComputerState.Color,
                    ComputerState.Config    => ComputerState.Turn,
                    ComputerState.Links     => ComputerState.Config,
                    ComputerState.Credits   => ComputerState.Links,
                    ComputerState.ERRORVIEW => ComputerState.Config,
                    _                       => ComputerState.Room
                };
                ResetPageCaches();
                UpdateScreen();
                return;
            }
            else if(buttonPressed.characterString == "down")
            {
                currentState = currentState switch{
                    ComputerState.Startup   => ComputerState.Room,
                    ComputerState.Room      => ComputerState.Name,
                    ComputerState.Name      => ComputerState.Color,
                    ComputerState.Color     => ComputerState.Turn,
                    ComputerState.Turn      => ComputerState.Config,
                    ComputerState.Config    => ComputerState.Links,
                    ComputerState.Links     => ComputerState.Credits,
                    ComputerState.Credits   => ComputerState.Room,
                    ComputerState.ERRORVIEW => ComputerState.Config,
                    _                       => ComputerState.Room
                };
                ResetPageCaches();
                UpdateScreen();
                return;
            }

            switch (currentState)
            {
                case ComputerState.Startup:
                    ProcessStartupState(buttonPressed);
                    break;
                case ComputerState.Color:
                    ProcessColorState(buttonPressed);
                    break;
                case ComputerState.Room:
                    ProcessRoomState(buttonPressed);
                    break;
                case ComputerState.Name:
                    ProcessNameState(buttonPressed);
                    break;
                case ComputerState.Turn:
                    ProcessTurnState(buttonPressed);
                    break;
                case ComputerState.Config:
                    ProcessConfigState(buttonPressed);
                    break;
                case ComputerState.Links:
                    ProcessLinksState(buttonPressed);
                    break;
                case ComputerState.Credits:
                    ProcessCreditsState(buttonPressed);
                    break;
                case ComputerState.ERRORVIEW:
                    ProcessErrorViewState(buttonPressed);
                    break;
            }

            UpdateScreen();
        }

        private void ResetPageCaches()
        {
            isChangingMods = false;
        }


        private void InitializeStartupState()
        {
        }
        private void InitializeRoomState()
        {
        }
        private void InitializeColorState()
        {
            redValue = PlayerPrefs.GetFloat("redValue", 0f);
            greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
            blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
            Debug.Log("GORILLA COMPUTER : PLAYERPREF COLORS RECALLED AS ; " + redValue*9 + ", " + greenValue*9 + ", " + blueValue*9);
            colorCursorLine = 0;
            foreach (Wardrobe wd in cosmeticsController.wardrobes)
            {
                if (wd.IsType<SatelliteWardrobeController>())
                {
                    SatelliteWardrobeController swd = wd.GetComponent<SatelliteWardrobeController>();
                    swd.colorSliderController.SetSliderValues(redValue, greenValue, blueValue);
                }
            }
            if(MiscGrabs.grab.DoesPlayerExist())
            {
                PlayerBase.i.GorillaVibration.UpdateColor(redValue, greenValue, blueValue);
            }
        }
        private void InitializeNameState()
        {
            savedName = PlayerPrefs.GetString("playerName", "gorilla");
            PhotonNetwork.LocalPlayer.NickName = savedName;
            currentName = savedName;
            exactOneWeek = exactOneWeekFile.text.Split('\n');
            anywhereOneWeek = anywhereOneWeekFile.text.Split('\n');
            anywhereTwoWeek = anywhereTwoWeekFile.text.Split('\n');
        }
        private void InitializeTurnState()
        {
            turnType = PlayerPrefs.GetString("Turning", "NONE");

            return;

            if (turnType == "SNAP")
            {
                turnProviderSnap.enabled = true;
                turnProviderSmooth.enabled = false;
            }
            else if (turnType == "SMOOTH")
            {
                turnProviderSnap.enabled = false;
                turnProviderSmooth.enabled = true;
            }
            else
            {
                turnProviderSnap.enabled = false;
                turnProviderSmooth.enabled = false;
            }
        }
        private void InitializeMicState()
        {
            pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
        }
        private void InitializeQueueState()
        {
            currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
            if (currentQueue != "DEFAULT" && currentQueue != "COMPETITIVE")
            {
                PlayerPrefs.SetString("currentQueue", "DEFAULT");
                PlayerPrefs.Save();
                currentQueue = "DEFAULT";
            }
        }
        private void InitializeSpawnState()
        {
            SpawnMapJoin = PlayerPrefs.GetString("SpawnMapJoin", "FOREST");
        }
        private void InitializeVoiceState()
        {
            voiceChatOn = PlayerPrefs.GetString("voiceChatOn", "TRUE");
        }
        private void InitializeGameMode()
        {
            /*currentGameMode = PlayerPrefs.GetString("currentGameMode", "INFECTION");
            if (currentGameMode != "CASUAL" && currentGameMode != "INFECTION" && currentGameMode != "HUNT")
            {
                PlayerPrefs.SetString("currentGameMode", "INFECTION");
                PlayerPrefs.Save();
                currentGameMode = "INFECTION";
            }*/
            //currentGameMode = PlayerPrefs.GetString("currentGameMode", "CASUAL");
            PlayerPrefs.SetString("currentGameMode", "CASUAL");
            PlayerPrefs.Save();
            currentGameMode = "CASUAL";
            OnModeSelectButtonPress(currentGameMode);
        }
        private void InitializeGameMod()
        {
            //currentGameMod = PlayerPrefs.GetString("currentGameMod", "STANDARD");
            //if (currentGameMod != "STANDARD" && currentGameMod != "NIGHTMARE" && currentGameMod != "SAFE")
            //{
                PlayerPrefs.SetString("currentGameMod", "STANDARD");
                PlayerPrefs.Save();
                currentGameMod = "STANDARD";
            //}
            //OnModSelectButtonPress(currentGameMod);
        }
        private void InitializeConfigState()
        {
            cnfgPSelect = 1;
            funcTextType = PlayerPrefs.GetString("funcTextType", "OLD");
            disableParticles = PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE";
            muteAllDuringEvents = PlayerPrefs.GetString("muteAllDuringEvents", "TRUE") == "FALSE";
            enablePostProcessing = PlayerPrefs.GetString("enablePostProcessing", "OFF") == "ON";
            postProcessingVolume.enabled = enablePostProcessing;
            LightCullDistance = PlayerPrefs.GetFloat("LightCullDistance", 0f);
            MirrorQualityLevel = PlayerPrefs.GetInt("MirrorQualityLevel", 1);
            showNametags = PlayerPrefs.GetString("showNametags", "FALSE") == "TRUE";
            savedWantedSpawn = PlayerPrefs.GetString("DesiredSpawn", "NULL");
            smoothFlashlightMotion = PlayerPrefs.GetInt("SmoothFlashlight", 10);
            flashlightSmoothingControl.SetSmoothing(smoothFlashlightMotion);
            nametagManager.visible = showNametags;
            lightCuller.Initialize(LightCullDistance);
            foreach (MirrorInstance mi in mirrorInstances)
            {
                mi.UpdateMirror(MirrorQualityLevel);
            }
            foreach(TerminalInstance t in Terminals)
            {
                if(t.Name == savedWantedSpawn)
                {
                    SetWantedSpawn(t);
                    break;
                }
            }
        }


        private void SwitchToColorState()
        {
            currentState = ComputerState.Color;
            blueText = Mathf.Floor(blueValue);
            redText = Mathf.Floor(redValue);
            greenText = Mathf.Floor(greenValue);
            UpdateScreen();
        }
        private void SwitchToRoomState()
        {
            currentState = ComputerState.Room;
            UpdateScreen();
        }
        private void SwitchToNameState()
        {
            currentState = ComputerState.Name;
            UpdateScreen();
        }
        private void SwitchToTurnState()
        {
            currentState = ComputerState.Turn;
            UpdateScreen();
        }
        private void SwitchToCreditsState()
        {
            currentState = ComputerState.Credits;
            UpdateScreen();
        }
        private void SwitchToConfigState()
        {
            currentState = ComputerState.Config;
            UpdateScreen();
        }
        private void SwitchToLinksState()
        {
            currentState = ComputerState.Links;
            UpdateScreen();
        }
        

        private void ProcessStartupState(GorillaKeyboardButton buttonPressed)
        {
            SwitchToRoomState();
        }
        private void ProcessColorState(GorillaKeyboardButton buttonPressed)
        {
            if (int.TryParse(buttonPressed.characterString, out var result))
            {
                //Debug.Log("GORILLA COMPUTER : COLOR RESULT RECIVED AS ; " + result + ", " + buttonPressed.characterString);
                //Debug.Log("GORILLA COMPUTER : COLOR TEXT BEFORE CHANGES ARE ; " + redText + ", " + greenText + ", " + blueText);
                //redText /= 9;greenText /= 9; blueText /= 9;
                //Debug.Log("GORILLA COMPUTER : COLOR TEXT MIDWAY CHANGES ARE ; " + redText + ", " + greenText + ", " + blueText);
                switch (colorCursorLine)
                {
                    case 0:
                        redText = result;
                        break;
                    case 1:
                        greenText = result;
                        break;
                    case 2:
                        blueText = result;
                        break;
                }
                //Debug.Log("GORILLA COMPUTER : COLOR TEXT IS NOW ; " + redText + ", " + greenText + ", " + blueText);
                redValue = redText;
                greenValue = greenText;
                blueValue = blueText;
                //Debug.Log("GORILLA COMPUTER : COLOR VALUE IS NOW ; " + redValue + ", " + greenValue + ", " + blueValue);
                PlayerPrefs.SetFloat("redValue", redValue);
                PlayerPrefs.SetFloat("greenValue", greenValue);
                PlayerPrefs.SetFloat("blueValue", blueValue);
                PlayerBase.i.GorillaVibration.UpdateColor(redValue, greenValue, blueValue);
                PlayerPrefs.Save();
                if (debug) {
                    Debug.Log("GORILLA COMPUTER : CURRENT COLORS ARE NOW ; " + redValue + ", " + greenValue + ", " + blueValue);
                }
                foreach (Wardrobe wd in cosmeticsController.wardrobes)
                {
                    if (wd.IsType<SatelliteWardrobeController>())
                    {
                        if (debug)
                        {
                            Debug.Log("GORILLA COMPUTER : SETTING COLOR SLIDERS FOR " + wd.gameObject, wd);
                        }
                        SatelliteWardrobeController swd = wd.GetComponent<SatelliteWardrobeController>();
                        swd.colorSliderController.SetSliderValues(redValue, greenValue, blueValue);
                    }
                }
                if (PhotonNetwork.InRoom)
                {
                    PlayerBase.i.GorillaVibration.myVRRig.photonView.RPC("SyncMaterial", RpcTarget.All, redValue, greenValue, blueValue);
                }
            }
            else
            {
                switch (buttonPressed.characterString)
                {
                    case "option1":
                        colorCursorLine = 0;
                        break;
                    case "option2":
                        colorCursorLine = 1;
                        break;
                    case "option3":
                        colorCursorLine = 2;
                        break;
                }
            }
        }
        public void ProcessNameState(GorillaKeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "enter":
                    if (currentName != savedName)
                    {
                        if (currentName != "" && CheckAutoBanList(currentName))
                        {
                            PhotonNetwork.LocalPlayer.NickName = currentName;
                        }
                        else
                        {
                            PhotonNetwork.LocalPlayer.NickName = "gorilla";
                            currentName = "gorilla";
                        }
                        PlayerBase.i.resources.playerNametag.text = currentName;
                        savedName = currentName;
                        PlayerPrefs.SetString("playerName", currentName);
                        PlayerPrefs.Save();
                        if (PhotonNetwork.InRoom)
                        {
                            PlayerBase.i.GorillaVibration.myVRRig.photonView.RPC("SyncMaterial", RpcTarget.All, redValue, greenValue, blueValue);
                        }
                    }
                    break;
                case "delete":
                    if (currentName.Length > 0)
                    {
                        currentName = currentName[..^1];
                    }
                    break;
                default:
                    if (currentName.Length < 12 && buttonPressed.characterString.Length == 1)
                    {
                        currentName += buttonPressed.characterString;
                    }
                    break;
            }
        }
        private void ProcessRoomState(GorillaKeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "enter":
                    if (roomToJoin != "")
                    {
                        networkController.AttemptToJoinSpecificRoom(roomToJoin);
                    }
                    break;

                case "delete":
                    if (roomToJoin.Length > 0)
                    {
                        roomToJoin = roomToJoin.Substring(0, roomToJoin.Length - 1);
                    }
                    break;

                case "option1":
                    if (!isChangingMods)
                    {
                        PhotonNetworkController.instance.AttemptDisconnect();
                    }
                    else
                    {
                        if (roomModSelect != 0) { roomModSelect = roomModSelect - 1; } else { roomModSelect = 2; }
                    }
                    break;

                case "option2":
                    if (isChangingMods)
                    {
                        if (roomModSelect != 2) { roomModSelect = roomModSelect + 1; } else { roomModSelect = 0; }
                    }
                    else
                    {
                        canShowFullMSG = false;
                    }
                        break;

                case "option3":
                    isChangingMods = !isChangingMods;
                    break;

                default:
                    if (roomToJoin.Length < 10)
                    {
                        roomToJoin += buttonPressed.characterString;
                    }
                    break;
            }
        }
        private void ProcessTurnState(GorillaKeyboardButton buttonPressed)
        {
            if (int.TryParse(buttonPressed.characterString, out var result))
            {
                turnValue = result;
                PlayerPrefs.SetInt("turnFactor", turnValue);
                PlayerPrefs.Save();
            }
            else
            {
                switch (buttonPressed.characterString)
                {
                    case "option1":
                        turnType = "SNAP";
                        PlayerPrefs.SetString("Turning", turnType);
                        PlayerPrefs.Save();
                        turnProviderSnap.gameObject.SetActive(true);
                        turnProviderSmooth.gameObject.SetActive(false);
                        break;
                    case "option2":
                        turnType = "SMOOTH";
                        PlayerPrefs.SetString("Turning", turnType);
                        PlayerPrefs.Save();
                        turnProviderSnap.gameObject.SetActive(false);
                        turnProviderSmooth.gameObject.SetActive(true);
                        break;
                    case "option3":
                        turnType = "NONE";
                        PlayerPrefs.SetString("Turning", turnType);
                        PlayerPrefs.Save();
                        turnProviderSnap.gameObject.SetActive(false);
                        turnProviderSmooth.gameObject.SetActive(false);
                        break;
                }
            }
        }
        private void ProcessConfigState(GorillaKeyboardButton buttonPressed)
        {
            
            switch (buttonPressed.characterString)
            {
                case "option1":
                    showCnfgInfo = false;
                    if(cnfgLSelect == 0)
                    {
                        cnfgLSelect = cnfgOptions[cnfgPSelect].Length-1;
                    }
                    else
                    {
                        cnfgLSelect--;
                    }
                    break;
                case "option2":
                switch (cnfgPSelect)
                    {
                        case 0:
                            switch (cnfgLSelect)
                            {
                                case 0:
                                    if(OverlayBuilder.instance)
                                        OverlayBuilder.instance.MainCanvas.SetActive(!OverlayBuilder.instance.MainCanvas.activeSelf);
                                    break;
                                case 1:
                                    if((playerCam.cullingMask & (1 << LayerMask.NameToLayer("DEV"))) == 0){playerCam.cullingMask |= 1 << LayerMask.NameToLayer("DEV");}
                                    else{playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("DEV"));}
                                    break;
                                case 2:
                                    if(OverlayBuilder.instance)
                                        OverlayBuilder.instance.DebugCanvas.SetActive(!OverlayBuilder.instance.DebugCanvas.activeSelf);
                                    break;
                                case 3:
                                    currentState = ComputerState.ERRORVIEW;
                                    break;
                                case 4:
                                    errorViewScroll = !errorViewScroll;
                                    break;
                            }
                            break;
                        case 1:
                            switch (cnfgLSelect)
                            {
                                case 0:
                                    disableParticles = !disableParticles;
                                    PlayerPrefs.SetString("disableParticles", disableParticles ? "TRUE" : "FALSE");
                                    break;
                                case 1:
                                    switch (funcTextType)
                                    {
                                        case "OLD":
                                            funcTextType = "SCROLL";
                                            PlayerPrefs.SetString("funcTextType", "SCROLL");
                                            break;
                                        case "SCROLL":
                                            funcTextType = "TOP";
                                            PlayerPrefs.SetString("funcTextType", "TOP");
                                            break;
                                        case "TOP":
                                            funcTextType = "CENTER";
                                            PlayerPrefs.SetString("funcTextType", "CENTER");
                                            break;
                                        case "CENTER":
                                            funcTextType = "OLD";
                                            PlayerPrefs.SetString("funcTextType", "OLD");
                                            break;
                                        default:
                                            funcTextType = "OLD";
                                            PlayerPrefs.SetString("funcTextType", "OLD");
                                            break;
                                    }
                                    UpdateFunctionScreen();
                                    break;
                                case 2:
                                    muteAllDuringEvents = !muteAllDuringEvents;
                                    PlayerPrefs.SetString("muteAllDuringEvents", muteAllDuringEvents ? "TRUE" : "FALSE");
                                    break;
                                case 3:
                                    enablePostProcessing = !enablePostProcessing;
                                    PlayerPrefs.SetString("enablePostProcessing", enablePostProcessing ? "ON" : "OFF");
                                    postProcessingVolume.enabled = enablePostProcessing;
                                    break;
                                case 4:
                                    showNametags = !showNametags;
                                    nametagManager.visible = showNametags;
                                    PlayerPrefs.SetString("showNametags", showNametags ? "TRUE" : "FALSE");
                                    break;
                                case 5:
                                    if(LightCullDistance == 0){LightCullDistance = 5;}
                                    else if (LightCullDistance == 5){LightCullDistance = 10;}
                                    else if (LightCullDistance == 10){LightCullDistance = 20;}
                                    else if (LightCullDistance == 20){LightCullDistance = 50;}
                                    else if (LightCullDistance == 50){LightCullDistance = 0;}
                                    lightCuller.Initialize(LightCullDistance);
                                    PlayerPrefs.SetFloat("LightCullDistance", LightCullDistance);
                                    break;
                                case 6:
                                    if(MirrorQualityLevel == 0){MirrorQualityLevel = 1;}
                                    else if (MirrorQualityLevel == 1){MirrorQualityLevel = 2;}
                                    else if (MirrorQualityLevel == 2){MirrorQualityLevel = 0;}
                                    PlayerPrefs.SetInt("MirrorQualityLevel", MirrorQualityLevel);
                                    foreach (MirrorInstance mi in mirrorInstances)
                                    {
                                        mi.UpdateMirror(MirrorQualityLevel);
                                    }
                                    break;
                            }
                    break;
                        case 2:
                            switch (cnfgLSelect)
                            {
                                case 0:
                                    TerminalInstance[] Termarray = Terminals.ToArray();
                                    int index = -1;
                                    
                                    for (int i =0; i<Termarray.Length;i++)
                                    {if(Termarray[i].Name == savedWantedSpawn){index = i;}}

                                    if(index >= Termarray.Length-1)
                                    {index = -1;} else {index++;}

                                    if(index == -1)
                                    {savedWantedSpawn = "NULL";} else {savedWantedSpawn = Termarray[index].Name;}

                                    PlayerPrefs.SetString("DesiredSpawn", savedWantedSpawn);

                                    break;
                                case 1:
                                    switch (smoothFlashlightMotion){
                                        case 0: smoothFlashlightMotion = 5; break;
                                        case 10: smoothFlashlightMotion = 25; break;
                                        case 25: smoothFlashlightMotion = 50; break;
                                        case 50: smoothFlashlightMotion = 0; break;
                                    }
                                    PlayerPrefs.SetInt("SmoothFlashlight", smoothFlashlightMotion);
                                    flashlightSmoothingControl.SetSmoothing(smoothFlashlightMotion);
                                    break;
                            }
                        break;
                    }
                    break;
                case "option3":
                    showCnfgInfo = false;
                    if(cnfgLSelect >= cnfgOptions[cnfgPSelect].Length-1)
                    {
                        cnfgLSelect = 0;
                    }
                    else
                    {
                        cnfgLSelect++;
                    }
                    break;
                case "I":
                    showCnfgInfo = !showCnfgInfo;
                    break;

                case "0":
                    cnfgPSelect = 0;
                    cnfgLSelect = 0;
                    break;
                case "1":
                    cnfgPSelect = 1;
                    cnfgLSelect = 0;
                    break;
                case "2":
                    cnfgPSelect = 2;
                    cnfgLSelect = 0;
                    break;
            }
            PlayerPrefs.Save();
        }
        private void ProcessLinksState(GorillaKeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "D":
                    Application.OpenURL(urls[0]);
                    break;
                case "F":
                    Application.OpenURL(urls[1]);
                    break;
                case "A":
                    Application.OpenURL(urls[2]);
                    break;
                case "I":
                    Application.OpenURL(urls[3]);
                    break;
                case "G":
                    Application.OpenURL(urls[4]);
                    break;
            }
        }
        private void ProcessCreditsState(GorillaKeyboardButton buttonPressed)
        {
        }
        private void ProcessErrorViewState(GorillaKeyboardButton buttonPressed)
        {
            switch (buttonPressed.characterString)
            {
                case "option1":
                    showCnfgErrorDetail = false;
                    cnfgSelectedErrorLog--;
                    if(cnfgSelectedErrorLog < 0) cnfgSelectedErrorLog = DebugLogs.Count-1;
                    break;
                case "option2":
                    showCnfgErrorDetail = !showCnfgErrorDetail;
                    break;
                case "option3":
                    showCnfgErrorDetail = false;
                    cnfgSelectedErrorLog++;
                    if(cnfgSelectedErrorLog > DebugLogs.Count-1) cnfgSelectedErrorLog = 0;
                    break;
                case "enter":
                    showCnfgErrorDetail = !showCnfgErrorDetail;
                    break;
                case "C":
                    cnfgSelectedErrorLog = 0;
                    DebugLogs.Clear();
                    break;
                case "delete":
                    currentState = ComputerState.Config;
                    break;
            }
        }

        private void UpdateScreen()
        {
            if (PhotonNetworkController.instance != null && !PhotonNetworkController.instance.wrongVersion)
            {
                UpdateFunctionScreen();
                switch (currentState)
                {
                    case ComputerState.Startup:
                        dataText = "GORILLA OS\n\n" + PhotonNetworkController.instance.TotalUsers() + " PLAYERS ONLINE\n\nPRESS ANY KEY TO BEGIN";
                        break;
                    case ComputerState.Color:
                        dataText = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
                        string text;
                        text = "\n\n  RED: " + Mathf.FloorToInt(redValue) + ((colorCursorLine == 0) ? "<--" : "");
                        string text2;
                        text2 = "\n\nGREEN: " + Mathf.FloorToInt(greenValue) + ((colorCursorLine == 1) ? "<--" : "");
                        string text3;
                        text3 = "\n\n BLUE: " + Mathf.FloorToInt(blueValue) + ((colorCursorLine == 2) ? "<--" : "");
                        dataText += text + text2 + text3;
                        break;
                    case ComputerState.Room:
                        {
                            if (!isChangingMods)
                            {
                                dataText = "OPTION 1: LEAVE / MOD DOWN. OPTION 2: CLEAR ERROR MSG / MOD UP. OPTION 3: VIEW ROOM MODS\n\nCURRENT ROOM: ";
                                if (PhotonNetwork.InRoom)
                                {
                                    dataText += PhotonNetwork.CurrentRoom.Name;
                                    string text4;
                                    text4 = "\n\nPLAYERS IN ROOM: " + PhotonNetwork.CurrentRoom.PlayerCount;
                                    dataText += text4;
                                }
                                else
                                {
                                    dataText += "-NOT IN ROOM-";
                                    string text5;
                                    text5 = "\n\nPLAYERS ONLINE: " + PhotonNetworkController.instance.TotalUsers();
                                    dataText += text5;
                                }
                                string text6;
                                text6 = "\n\nROOM TO JOIN: " + roomToJoin;
                                dataText += text6;
                                if (roomFull && canShowFullMSG)
                                {
                                    dataText += "\n\nROOM FULL. JOIN ROOM FAILED.";
                                }
                                //foreach(GorillaTerminal gt in terminals)
                                {
                                    //gt.Image.gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                //string textmod;
                                dataText = "ROOM MODS GET APPLIED PER NEW ROOM, MODS WILL ADJUST WHEN JOINING ROOMS\n\nCURRENT ROOM: ";
                                if (PhotonNetwork.InRoom)
                                {
                                    dataText += PhotonNetwork.CurrentRoom.Name;
                                    string text4;
                                    text4 = "\n\nPLAYERS IN ROOM: " + PhotonNetwork.CurrentRoom.PlayerCount;
                                    dataText += text4;
                                    dataText += "\n\nCURRENT ROOM MOD = " + currentGameMod;
                                }
                                else
                                {
                                    dataText += "-NOT IN ROOM-";
                                    dataText += "\n\n OK SO THIS WORKS BUT I'M NOT GONNA DO ANYTHING WITH IT. BETTER LUCK NEXT TIME.";
                                    //currentRoomMod2 = "0: STANDARD";
                                    //dataText += "\n\nCURRENT ROOM MOD = " + "-NOT IN ROOM-";
                                }
                                /*switch (roomModSelect)
                                {
                                    case 0:
                                        textmod = "0: STANDARD";
                                        dataText += "\n\nROOM MOD = " + textmod + "";
                                        break;
                                    case 1:
                                        textmod = "1: NIGHTMARE";
                                        dataText += "\n\nROOM MOD = " + textmod + "";
                                        break;
                                    case 2:
                                        textmod = "2: SAFE (TIMED)";
                                        dataText += "\n\nROOM MOD = " + textmod + "";
                                        break;
                                    default:
                                        roomModSelect = 0;
                                        textmod = "0: STANDARD";
                                        dataText += "\n\nROOM MOD = " + textmod + "";
                                        break;

                                }
                                //foreach (GorillaTerminal gt in terminals)
                                {
                                    //i.gameObject.SetActive(true);
                                }*/


                            }

                            break;
                        }
                    case ComputerState.Name:

                        string text7 = "";
                        dataText = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\nCURRENT NAME: " + savedName;
                        if (currentName.Contains("go") || savedName.Contains("go"))
                        {
                            dataText = "CHANGE YOUR NAME TO GAIN ACCESS TO SERVERS.\n\n";
                        }
                        text7 = text7 + "\n\n    " + (currentName.Contains("go") ? "NAME: " : "NEW NAME: ") + currentName;
                        dataText += text7;
                        break;
                    case ComputerState.Turn:
                        dataText = "PRESS OPTION 1 TO TURN ON SNAP TURN.\n PRESS OPTION 2 TO TURN ON SMOOTH TURN.\n PRESS OPTION 3 TO TURN OFF TURNING. \n\n CURRENT TURN TYPE: " + turnType;
                        break;
                    case ComputerState.Config:
                        configText = "";
                        
                        if (showCnfgInfo)
                        {
                            configText = "\n" + GetOptionInfo() + "\n CURRENT SETTING IS: " + GetOptionCurrentSetting(cnfgPSelect, cnfgLSelect);
                        }
                        else
                        {
                            for(int i = 0; i < cnfgOptions[cnfgPSelect].Length; i++)
                            {
                                configText += "\n" + ((i == cnfgLSelect) ? " ->" : "  ") + cnfgOptions[cnfgPSelect][i];
                                configText += GetOptionCurrentSetting(cnfgPSelect, i);
                            }
                        }

                        dataText = dateNTime + ( showCnfgInfo ? "  \\  PRESS I TO EXIT\n" : "  \\  PRESS I FOR INFO\n" ) +
                         "PAGE: " + (cnfgPSelect == 0 ? " [DEV] " : "") + (cnfgPSelect == 1 ? " [1] " : " 1 ") + (cnfgPSelect == 2 ? " [2] " : " 2 ") + configText;

                        break;
                    case ComputerState.Credits:
                        dataText = "CREDITS\n\n GT BY ANOTHER AXIOM\n\n GTH BY LEMMON\n\n REMIX BY LAPS";
                        break;
                    case ComputerState.Links:
                        dataText = " D: OFFICIAL GTH REMIX DISCORD\n A: GTH REMIX APPLAB\n F: ARCHIVE DISCORD\n I: REMIX ITCH PAGE\n G: REMIX RESOURCES";
                        break;
                    case ComputerState.ERRORVIEW:
                        dataText = !showCnfgErrorDetail ? GetErrorViewDisplay() : $"[{cnfgSelectedErrorLog+1}]" + " " + DebugLogs[cnfgSelectedErrorLog].stack;
                        break;
                }
                foreach (Text t in screenTexts)
                {
                    t.text = dataText;
                }
            }
        }
        private void UpdateFunctionScreen()
        {
            funcText = funcTextType switch
            {
                "CENTER" => ((currentState == ComputerState.Room)               ? "cnfg \nlinks \ncrdts \nROOM <- \nname \ncolor \nturn" : "")
                                    + ((currentState == ComputerState.Name)     ? "links \ncrdts \nroom \nNAME <- \ncolor \nturn \ncnfg" : "")
                                    + ((currentState == ComputerState.Color)    ? "crdts \nroom \nname \nCOLOR <- \nturn \ncnfg \nlinks" : "")
                                    + ((currentState == ComputerState.Turn)     ? "crdts \nroom \nname \ncolor \nTURN <- \ncnfg \nlinks" : "")
                                    + ((currentState == ComputerState.Config)   ? "name \ncolor \nturn \nCNFG <- \nlinks \ncrdts \nroom" : "")
                                    + ((currentState == ComputerState.Links)    ? "color \nturn \ncnfg \nLINKS <- \ncrdts \nroom \nname" : "")
                                    + ((currentState == ComputerState.Credits)  ? "Spawn \ncnfg \nlinks \nCRDTS <- \nroom \nname \ncolor \nturn" : ""),
                "TOP" => ((currentState == ComputerState.Room)                      ? "ROOM <-\nname \ncolor \nturn \ncnfg \nlinks \ncrdts" : "")
                                    + ((currentState == ComputerState.Name)         ? "NAME <- \ncolor \nturn \ncnfg \nlinks \ncrdts \nroom" : "")
                                    + ((currentState == ComputerState.Color)        ? "COLOR <- \nturn \ncnfg \nlinks \ncrdts \nroom \nname" : "")
                                    + ((currentState == ComputerState.Turn)         ? "TURN <- \ncnfg \nlinks \ncrdts \nroom \nname \ncolor" : "")
                                    + ((currentState == ComputerState.Config)       ? "CNFG <- \nlinks  \ncrdts \nroom \nname \ncolor \nturn" : "")
                                    + ((currentState == ComputerState.Links)        ? "LINKS <- \ncrdts \nroom \nname \ncolor \nturn \ncnfg" : "")
                                    + ((currentState == ComputerState.Credits)      ? "CRDTS <- \nroom \nname \ncolor \nturn \ncnfg \nlinks" : ""),
                "SCROLL" => ((currentState == ComputerState.Room)               ? "ROOM <-" : "room")
                                    + ((currentState == ComputerState.Name)     ? "\nNAME <-" : "\nname")
                                    + ((currentState == ComputerState.Color)    ? "\nCOLOR <-" : "\ncolor")
                                    + ((currentState == ComputerState.Turn)     ? "\nTURN <-" : "\nturn")
                                    + ((currentState == ComputerState.Config)   ? "\nCNFG <-" : "\ncnfg")
                                    + ((currentState == ComputerState.Links)    ? "\nLINKS <-" : "\nlinks")
                                    + ((currentState == ComputerState.Credits)  ? "\nCRDTS <-" : "\ncrdts"),
                _ => "ROOM" + ((currentState == ComputerState.Room) ? "<-" : "")
                                    + "\nNAME " + ((currentState == ComputerState.Name) ? "<-" : "")
                                    + "\nCOLOR" + ((currentState == ComputerState.Color) ? "<-" : "")
                                    + "\nTURN " + ((currentState == ComputerState.Turn) ? "<-" : "")
                                    + "\nCNFG" + ((currentState == ComputerState.Config) ? "<-" : "")
                                    + "\nLINKS " + ((currentState == ComputerState.Links) ? "<-" : "")
                                    + "\nCRDTS" + ((currentState == ComputerState.Credits) ? "<-" : ""),
            };
            if (functionSelectTexts.Count != 0){
            foreach (Text t in functionSelectTexts)
            {
                t.text = funcText;
            }}
        }
        public void UpdateRoomScreens()
        {
            foreach (RegMapSegment rms in RegisteredMaps)
            {
                if (currentRoomGeoLoco == rms.MapName)
                {
                    foreach (GorillaLevelScreen gls in rms.joinScreens)
                    {
                        gls.ReveluateStatus(true);
                    }
                    foreach (GorillaLevelScreen gls in rms.leaveScreens)
                    {
                        gls.ReveluateStatus(false);
                    }
                }
            }
        }

        public string GetOptionInfo()
        {
            if(cnfgInfo[cnfgPSelect][cnfgLSelect] != null)
            {
                return cnfgInfo[cnfgPSelect][cnfgLSelect];
            }
            else
            {
                return "NO ADDITIONAL INFO WAS PROVIDED FOR THIS SETTING.";
            }
        }
        public string GetOptionCurrentSetting(int Page, int Option)
        {
            string returnString = "SETTING NOT FOUND";
            switch (Page)
            {
                case 0:
                    switch (Option)
                    {
                        case 0:
                            returnString = OverlayBuilder.instance.MainCanvas.activeSelf ? "TRUE" : "FALSE";
                            break;
                        case 1:
                            returnString = (playerCam.cullingMask & (1 << LayerMask.NameToLayer("DEV"))) != 0 ? "TRUE" : "FALSE";
                            break;
                        case 2:
                            returnString = OverlayBuilder.instance.DebugCanvas.activeSelf ? "TRUE" : "FALSE";
                            break;
                        case 3:
                            returnString = "";
                            break;
                        case 4:
                            returnString = errorViewScroll ? "TRUE" : "FALSE";
                            break;
                    }
                    break;
                case 1:
                    switch (Option)
                    {
                        case 0:
                            returnString = disableParticles ? "TRUE" : "FALSE";
                            break;
                        case 1:
                            returnString = funcTextType;
                            break;
                        case 2:
                            returnString = muteAllDuringEvents ? "TRUE" : "FALSE";
                            break;
                        case 3:
                            returnString = enablePostProcessing ? "ON" : "OFF";
                            break;
                        case 4:
                            returnString = showNametags ? "TRUE" : "FALSE";
                            break;
                        case 5:
                            returnString = (LightCullDistance == 0) ? "DISABLED" : LightCullDistance.ToString();
                            break;
                        case 6:
                            returnString = MirrorQualityLevel.ToString();
                            break;
                    }
                    break;
                case 2:
                    switch (Option)
                    {
                        case 0:
                            returnString = savedWantedSpawn;
                            break;
                        case 1:
                            returnString = (smoothFlashlightMotion == 0) ? "DISABLED" : smoothFlashlightMotion.ToString();
                            break;
                    }
                    break;
            }
            return returnString;
        }
        public string GetErrorViewDisplay()
        {
            string conjoin = "";
            if(errorViewScroll){
                if(cnfgSelectedErrorLog + 5 > DebugLogs.Count - 1)
                {
                    if(DebugLogs.Count-5 < 1)
                        for(int i = 0; i < DebugLogs.Count; i++)
                        {
                            if(i == cnfgSelectedErrorLog) conjoin += $"[{cnfgSelectedErrorLog+1}/{DebugLogs.Count}]" + "->";
                            conjoin += GetColoredLogContent(DebugLogs[i]) + "\n";
                        }
                    else if(DebugLogs.Count != 0)
                        for(int i = DebugLogs.Count-5; i < DebugLogs.Count; i++)
                        {
                            if(i == cnfgSelectedErrorLog) conjoin += $"[{cnfgSelectedErrorLog+1}/{DebugLogs.Count}]" + "->";
                            conjoin += GetColoredLogContent(DebugLogs[i]) + "\n";
                        }
                    else
                        return "NO LOGS TO PRESENT";
                }
                else
                {
                    conjoin += $"[{cnfgSelectedErrorLog+1}/{DebugLogs.Count}]" + "->";
                    for(int i = cnfgSelectedErrorLog; i < cnfgSelectedErrorLog+5; i++)
                    {
                        conjoin += GetColoredLogContent(DebugLogs[i]) + "\n";
                    }
                }
            }
            else
            {
                if(DebugLogs.Count != 0)
                    conjoin = $"[{cnfgSelectedErrorLog+1}/{DebugLogs.Count}]" + "-> " + GetColoredLogContent(DebugLogs[cnfgSelectedErrorLog]);
                else
                    return "NO LOGS TO PRESENT";
            }
            return conjoin;
        }
        private string GetColoredLogContent(LogDisplay ld)
        {
            return LoggingConversion.IntToRichStringColor(ld.severity) + ld.content + "</color>"; 
        }

        private bool CheckAutoBanList(string nameToCheck)
        {
            nameToCheck = nameToCheck.ToLower();
            string[] array = anywhereTwoWeek;
            foreach (string text in array)
            {
                if (nameToCheck.IndexOf(text.ToLower().TrimEnd('\r', '\n')) >= 0)
                {
                    BanMe(336, nameToCheck);
                    return false;
                }
            }
            array = anywhereOneWeek;
            foreach (string text2 in array)
            {
                if (nameToCheck.IndexOf(text2.ToLower().TrimEnd('\r', '\n')) >= 0 && !nameToCheck.Contains("fagol"))
                {
                    BanMe(168, nameToCheck);
                    return false;
                }
            }
            array = exactOneWeek;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ToLower().TrimEnd('\r', '\n') == nameToCheck)
                {
                    BanMe(168, nameToCheck);
                    return false;
                }
            }
            return true;
        }
        private void BanMe(int hours, string nameToCheck)
        {
            Application.Quit();
            Startup.CallIt();
        }
        public void GeneralFailureMessage(string failMessage)
        {
            conectivityState = ConectivityState.Banned;
            //dataText = failMessage;
            foreach (Text t in screenTexts)
            {
                t.text = dataText;
            }
            foreach (GorillaLevelScreen gls in MiscGrabs.grab.LevelScreens)
				{
					gls.UpdateText(failMessage, setToGoodMaterial: false);
				}
            isConnectedToMaster = false;
            networkController.WrongVersion();
            UpdateScreen();
        }
        public void RefreshRigColor()
        {
            if(MiscGrabs.grab.DoesPlayerExist())
            {
                PlayerBase.i.GorillaVibration.UpdateColor(redValue, greenValue, blueValue);
            }
        }
        public void UpdateConectivityStatus(ConectivityState State)
        {
            conectivityState = State;
            UpdateScreen();
        }
        private void SetWantedSpawn(TerminalInstance t)
        {
            networkController.SetPlayerPosition(t.Spwn.position);
        }
        public void OnModeSelectButtonPress(string gameMode)
        {
            currentGameMode = gameMode;
            PlayerPrefs.SetString("currentGameMode", gameMode);
            PlayerPrefs.Save();
            ModeSelectButton[] array = modeSelectButtons;
            if(array.Length != 0)
            foreach (ModeSelectButton modeSelectButton in array)
            {
                modeSelectButton.buttonRenderer.material = (currentGameMode == modeSelectButton.gameMode) ? modeSelectButton.pressedMaterial : modeSelectButton.unpressedMaterial;
            }
        }
        public string GetRoomModString()
        {
            return roomModSelect switch
            {
                0 => "NORM",
                1 => "NIGHTMARE",
                2 => "SAFE",
                _ => "ERROR",
            };
        }
        public void SetGameModInt(int rm)
        {
            switch (rm)
            {
                case 0:
                    currentGameMod = "0: STANDARD";
                    break;
                case 1:
                    currentGameMod = "1: NIGHTMARE";
                    break;
                case 2:
                    currentGameMod = "2: SAFE (TIMED)";
                    break;
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            LogDisplay nld = new() { content = logString, stack = stackTrace, lifetime = 0f, severity = LoggingConversion.LogTypeToInt(type) };

            DebugLogs.Add(nld);
        }

        public void OnDestroy()
        {
            Application.logMessageReceived -= Log;
        }

        /*public void AddTerminal(Text Data, Text FuncSelect, Image image = null)
        {
            GorillaTerminal tta = null;

            tta.Data = Data;
            tta.FuncSelect = FuncSelect;
            if (image != null) { tta.Image = image; }

            terminals.Add(tta);
        }*/

        public void SetColors(float RedNew, float GreenNew, float BlueNew)
        {
            redValue = RedNew;
            greenValue = GreenNew;
            blueValue = BlueNew;
            Debug.Log("GORILLA COMPUTER : RECIVED COLORS FROM WARDROBE AS; " + redValue + ", " + greenValue + ", " + blueValue);
            PlayerPrefs.SetFloat("redValue", redValue);
            PlayerPrefs.SetFloat("greenValue", greenValue);
            PlayerPrefs.SetFloat("blueValue", blueValue);
            PlayerBase.i.GorillaVibration.UpdateColor(redValue, greenValue, blueValue);
            PlayerPrefs.Save();

            Debug.Log("GORILLA COMPUTER : SETTING WARDROBE COLOR SLIDERS AS; " + RedNew + ", " + GreenNew + ", " + BlueNew);
            cosmeticsController.UpdateWardrobeColorSliders(RedNew, GreenNew, BlueNew);

            if (PhotonNetwork.InRoom)
            {
                PlayerBase.i.GorillaVibration.myVRRig.photonView.RPC("SyncMaterial", RpcTarget.All, redValue/9, greenValue/9, blueValue/9);
            }

            if (currentState == ComputerState.Color)
            {
                UpdateScreen();
            }
        }
        public void RefreshWardrobeSliders()
        {
            cosmeticsController.UpdateWardrobeColorSliders(redValue, greenValue, blueValue);
        }
        public void AddTerminal(TerminalInstance Terminal)
        {
            screenTexts.Add(Terminal.Data);
            functionSelectTexts.Add(Terminal.FuncSelect);
            Terminals.Add(Terminal);
            UpdateScreen();
        }
        public string FetchMODTString(string url)
        {
            if (fetchedMOTD) return MOTDText;

            StartCoroutine(WebUtilities.DownloadTextFromURL(url, (text) => { MOTDText = text; }));
            return MOTDText;
        }
    }
}
