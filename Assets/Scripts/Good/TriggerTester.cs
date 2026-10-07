using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTester : RemixScript
{
    public bool activate;
    public UnityEvent events;

    public void Update()
    {
        if (activate)
        {
            events.Invoke();
            activate = false;
        }
    }
}
