using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxSwap : RemixScript
{
    public Material skyToSwap;

    /*public bool trigger;

    public void Update()
    {
        if (trigger)
        {
            trigger = false;
            MiscGrabs.grab.CameraSkybox2.material = skyToSwap;
        }
    }*/

    public void OnTriggerEnter(Collider other)
    {
        MiscGrabs.grab.SkyManager.ChangeSkybox(skyToSwap);
    }
}
