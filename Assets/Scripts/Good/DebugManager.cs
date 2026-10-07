using GorillaNetworking;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
#pragma warning disable UDR0001 // Domain Reload Analyzer
    public static volatile DebugManager instance;
#pragma warning restore UDR0001 // Domain Reload Analyzer

    public bool isAndroid;
    public bool isDesktop;
    public bool initialized;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(base.gameObject);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            isAndroid = true;

            if(!initialized && isAndroid)
            {
                PhotonNetworkController.instance.debug = false;
                initialized = true;
            }
        }
        else
        {
            if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                isDesktop = true;
            }
            PhotonNetworkController.instance.debug = true;
            initialized = true;
        }
    }
}
