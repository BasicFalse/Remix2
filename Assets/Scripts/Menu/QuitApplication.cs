using System.Collections;
using System.Collections.Generic;
using UFunction;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GorillaPlayer")
        {
            Application.Quit();
            Startup.CallIt();
        }
    }
}