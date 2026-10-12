using GravNew;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public static PlayerBase @i;
    public PlayerType currentPlayerType;

    public PlayerTeleport RTeleport;
    public PlayerJoystick RJoystick;
    public PlayerPhysical RPhysical;
    public GorillaVibration GorillaVibration;
    public NormalisedGravity gravity;

    [SerializeField]
    public ResourcesContainer resources;
    public TransformContainer transforms;
    public BoolContainer bools;
    public ColliderContainer colliders;

    public float getSizeRatio
    {
        get
        {
            return sizeRatio;
        }
    }

    private float sizeRatio
    {
        get
        {
            return transforms.PlayerRoot.localScale.x;
        }
    }

    void Awake()
    {
        if (@i == null)
            @i = this;
        else    
            Debug.Log("DUPLICATE PLAYERBASE");
    }

    public void Initialize()
    {
        SwitchPlayerType(StringToPlayerType(PlayerPrefs.GetString("PlayerTypePrefrence", "Physical")));
    }

    public void Update()
    {
        switch (currentPlayerType){
            case PlayerType.Teleport:
                RTeleport.UpdatePlayer();
            break;
            case PlayerType.Joystick:
                RJoystick.UpdatePlayer();
            break;
            case PlayerType.Physical:
                RPhysical.UpdatePlayer();
            break;
        }
    }

    public void FixedUpdate()
    {
        gravity.UpdateGravity();
    }

    public PlayerType StringToPlayerType(string stt)
    {
        switch (stt)
        {
            case "Teleport":
                return PlayerType.Teleport;
            case "Joystick":
                return PlayerType.Joystick;
            case "Physical":
                return PlayerType.Physical;
            default:
                Debug.Log("STRING TO PLAYER TYPE INVALID");
                return PlayerType.Invalid;
        }
    }

    public void EmergencyTeleport(int type)
    {
        switch (type)
        {
            case 0:
                transform.position = MiscGrabs.grab.SM.CurrentSpawnDat().position;
            break;
            case 1:
                transform.position = MiscGrabs.grab.SM.CurrentSpawnDat().position;
            break;
        }
        resources.RB.linearVelocity = new();
    }

    public void SwitchPlayerType(PlayerType pt)
    {
        if(RTeleport)
        RTeleport.enabled = false;
        if(RJoystick)
        RJoystick.enabled = false;
        if(RPhysical)
        RPhysical.enabled = false;
        currentPlayerType = pt;
        switch (pt)
        {
            case PlayerType.Teleport:
                RTeleport.enabled = true;
                SwitchToTypeTeleport();
            break;
            case PlayerType.Joystick:
                RJoystick.enabled = true;
                SwitchToTypeJoystick();
            break;
            case PlayerType.Physical:
                RPhysical.enabled = true;
                SwitchToTypePhysical();
            break;
        }
    }

    public void SwitchToTypeTeleport()
    {
    }

    public void SwitchToTypeJoystick()
    {
    }

    public void SwitchToTypePhysical()
    {
    }
}
