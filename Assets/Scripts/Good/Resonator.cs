using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Resonator : MonoBehaviour
{
    public bool didInitiateProtocol;
    public bool isPlayerInResonator;
    public bool isResonatorActive;
    public float timepassed = 0;
    public float timetowait = 2f;
    public MeshRenderer buttonMesh;
    public MeshRenderer resonatorMesh;
    public MeshRenderer doorT;
    public MeshRenderer doorB;
    public GorillaPressableButton button;
    public Text[] screens;
    public DataHolder data;
    public Material buttonBright;
    public Material buttonDim;
    public Material innerScreen;
    public Animator DoorAnim;

    public void ChangeDoorState(bool open)
    {
        if (open)
        { DoorAnim.SetTrigger("Open"); }
        else
        { DoorAnim.SetTrigger("Close"); }

    }

    public void ButtonPressed()
    {
        if (!isResonatorActive)
        {
            buttonMesh.material = buttonDim;
            isResonatorActive = true;
            resonatorMesh.materials[2].SetFloat("_Intensity", 0.2f);
            doorT.materials[1].SetFloat("_Intensity", 0.2f);
            doorB.materials[1].SetFloat("_Intensity", 0.2f);
            ChangeDoorState(true);
        }
    }
    
    public void PlayerEnterResonator()
    {
        if (!isPlayerInResonator)
        {
            isPlayerInResonator = true;
            ChangeDoorState(false);
            PlayerPrefs.SetString("resonator", "done");
            PlayerPrefs.Save();
        }
    }

    public void FixedUpdate(){

        if (isResonatorActive && isPlayerInResonator)
        {
            timepassed += 1f * Time.deltaTime;
            if (timepassed < 0.2f)
            {
                resonatorMesh.materials[2].SetFloat("_Intensity", 0.2f);
                doorT.materials[1].SetFloat("_Intensity", 0.2f);
            doorB.materials[1].SetFloat("_Intensity", 0.2f);
            }
            else
            {
                resonatorMesh.materials[2].SetFloat("_Intensity", timepassed);
                doorT.materials[1].SetFloat("_Intensity", timepassed);
                doorB.materials[1].SetFloat("_Intensity", timepassed);
            }
        }
        
        if(timepassed > timetowait)
        {
            resonatorMesh.materials[2].SetFloat("_Intensity", 0);
            doorT.materials[1].SetFloat("_Intensity", 0);
            doorB.materials[1].SetFloat("_Intensity", 0);
            Application.Quit();
            Debug.Log("did it quit yet?");
        }
    }
}
