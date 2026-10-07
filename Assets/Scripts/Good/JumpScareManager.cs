using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Diagnostics;

public class JumpScareManager : RemixScript
{
    public enum JumpscareSetting
    {
        Classic, Gradient
    }
    public enum JumpscareType
    {
        Timmy, TimmyCanyons, Jimmy, JimmyLava, BroGotTased, Bimby, Fly, Spiderv1, Spiderv2, Spiderv3, Yeti, CaseOh, CrateMimic
    }

    public static volatile JumpScareManager jump;

    [Header("Classic Jumpscares")]

    public GameObject TimmyJump;
    public GameObject TimmyCanyonsJump;
    public GameObject JimmyJump;
    public GameObject JimmyLavaJump;
    public GameObject TasedJump;
    public GameObject BimbyJump;
    public GameObject FlyJump;
    public GameObject Spiderv1Jump;
    public GameObject Spiderv2Jump;
    public GameObject Spiderv3Jump;
    public GameObject YetiJump;
    public GameObject CaseOhJump;
    public GameObject CrateMimicJump;

    [Space]

    [Header("Coustom Jumpscares")]

    public MeshRenderer ColorJumper;

    //private Hashtable Callers;


    private void Awake()
    {
        if (jump == null) { jump = this; } else if (jump != this) { gameObject.SetActive(false); }
    }

    public void CallJumpscare(JumpscareType JST, float waitTime, Jumpscare2Point2 js)
    {
        if (UnityEngine.Application.platform != RuntimePlatform.Android) { return; }
        switch (JST)
        {
            case JumpscareType.Timmy:
                TimmyJump.SetActive(true);
                break;
            case JumpscareType.TimmyCanyons:
                TimmyCanyonsJump.SetActive(true);
                break;
            case JumpscareType.Jimmy:
                JimmyJump.SetActive(true);
                break;
            case JumpscareType.JimmyLava:
                JimmyLavaJump.SetActive(true);
                break;
            case JumpscareType.BroGotTased:
                TasedJump.SetActive(true);
                break;
            case JumpscareType.Bimby:
                BimbyJump.SetActive(true);
                break;
            case JumpscareType.Fly:
                FlyJump.SetActive(true);
                break;
            case JumpscareType.Spiderv1:
                Spiderv1Jump.SetActive(true);
                break;
            case JumpscareType.Spiderv2:
                Spiderv2Jump.SetActive(true);
                break;
            case JumpscareType.Spiderv3:
                Spiderv3Jump.SetActive(true);
                break;
            case JumpscareType.Yeti:
                YetiJump.SetActive(true);
                break;
            case JumpscareType.CaseOh:
                CaseOhJump.SetActive(true);
                break;
            case JumpscareType.CrateMimic:
                CrateMimicJump.SetActive(true);
                break;
        }
        StartCoroutine(QuitGame(waitTime, js));
    }

    public void CallCustomJumpscare(Gradient gradient, float lerpValue)
    {
        StartCoroutine(GradientMaterial(gradient, ColorJumper, lerpValue));
    }

    IEnumerator QuitGame(float wt, Jumpscare2Point2 js)
    {
        yield return new WaitForSeconds(wt/2);
        PhotonNetworkController.instance.AttemptDisconnect();
        yield return new WaitForSeconds(wt/2);
        UnityEngine.Application.Quit();
        Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
        Utils.NativeError("Packet error: 0x80000000 | Undefined");
        System.Diagnostics.Process.GetCurrentProcess().Kill();
        System.Environment.Exit(0);
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity?.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {                currentActivity.Call("finishAffinity");
            }));
    }

    static IEnumerator GradientMaterial(Gradient gradient, MeshRenderer mr, float duration)
    {
        float elapsed = 0f;
    
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
    
            Color color = gradient.Evaluate(t);
            mr.material.color = color;
    
            yield return null;
        }
    
        Color final = gradient.Evaluate(1f);
        mr.material.color = final;
    }
}