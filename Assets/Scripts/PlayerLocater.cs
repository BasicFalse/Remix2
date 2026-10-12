using GorillaNetworking;
using UnityEngine;

public class PlayerLocater : MonoBehaviour
{
    void FixedUpdate()
    {
        if (MiscGrabs.grab != null)
        {
            transform.position = MiscGrabs.grab.GetPlayerPosition();
        }
    }

    void Update()
    {
        if (MiscGrabs.grab != null)
        {
            transform.position = MiscGrabs.grab.GetPlayerPosition();
        }
    }
}
