using System.Collections;
using ExitGames.Client.Photon.StructWrapping;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Diagnostics;

public class JumpscareVisuals : MonoBehaviour
{
    public static volatile JumpscareVisuals @i;
    public JumpscareRepo jumpscareRepo;
    public Transform jumpscareParent;
    public GameObject jumpCache;
    

    public void Awake()
    {
        if (@i == null)
        {
            @i = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void CallJumpscare(string id, int type = 0)
    {
        //if(type != 0) return;
        GameObject prefab = jumpscareRepo.GetJumpscarePrefab(id);
        if (prefab != null && jumpCache == null)
        {
            InstanceJumpscare(prefab);
        }
        StartCoroutine(QuitGame(2f));
        
    }

    private void InstanceJumpscare(GameObject prefab)
    {
        if (prefab != null && jumpCache == null)
        {
            jumpCache = Instantiate(prefab, jumpscareParent.position, jumpscareParent.rotation, jumpscareParent);
            jumpCache.transform.localPosition = Vector3.zero;
            jumpCache.transform.localRotation = Quaternion.identity;
        }
    }

    IEnumerator QuitGame(float wt)
    {
        yield return new WaitForSeconds(wt/5*4);
        PhotonNetworkController.instance.AttemptDisconnect();
        yield return new WaitForSeconds(wt/5);
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

#if UNITY_EDITOR
    public bool testJumpscare = false;
    public string testJumpscareID = "Timmy";

    private void Update()
    {
        if (!testJumpscare) return;

        if(jumpscareRepo.GetJumpscarePrefab(testJumpscareID) == null && jumpCache != null)
        {
            Destroy(jumpCache);
            jumpCache = null;
        }
        else if (jumpscareRepo.GetJumpscarePrefab(testJumpscareID) != null && jumpCache == null)
        {
            InstanceJumpscare(jumpscareRepo.GetJumpscarePrefab(testJumpscareID));
        }

        if(jumpCache == null ) return;
    }

#endif
}