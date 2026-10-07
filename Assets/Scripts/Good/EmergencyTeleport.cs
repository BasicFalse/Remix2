using UnityEngine;

public class EmergencyTeleport : MonoBehaviour
{
    public int type = 0;
    private void OnTriggerEnter(Collider other)
    {
        PlayerBase.i.EmergencyTeleport(type);
    }
}
