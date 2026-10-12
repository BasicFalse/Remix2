using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    public PlayerBase playerBase;
    public GorillaVibration gorillaVibration;

    public void WakeNewPlayer()
    {
        playerBase.Initialize();
        gorillaVibration.Initialize();
    }
}
