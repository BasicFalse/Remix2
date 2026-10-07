using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : RemixScript
{
    public bool isOn;
    public bool isQuiet;

    public AudioSource audioSource;

    public Material pressedMat;
    public Material powerMat;
    public Material volMat;

    public MeshRenderer buttonPower;
    public MeshRenderer buttonVol;

    public RadioButton powerButton;
    public RadioButton volButton;

    public override void StartExt()
    {
        powerButton.On = true;
    }

    public void Update()
    {
        isOn = powerButton.On;
        isQuiet = volButton.On;
        if (isOn)
        {
            buttonPower.material = pressedMat;
            audioSource.mute = false;
        }
        else
        {
            buttonPower.material = powerMat;
            audioSource.mute = true;
        }
        if (isQuiet)
        {
            buttonVol.material = pressedMat;
            audioSource.volume = 0.4f;
        }
        else
        {
            buttonVol.material = volMat;
            audioSource.volume = 0.7f;
        }
    }
}
