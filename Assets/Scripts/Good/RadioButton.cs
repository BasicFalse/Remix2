using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioButton : RemixScript
{
    public bool trigger;
    public bool On;

    private void Update()
    {
        if (trigger)
        {
            On = !On;
            trigger = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
            On = !On;
    }
}
