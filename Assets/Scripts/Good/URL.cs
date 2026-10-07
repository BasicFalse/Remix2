using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL : RemixScript
{
    private void OnTriggerEnter(Collider other)
    {
        Application.OpenURL("https://laps-labs.itch.io/gth-remix");
    }
}
