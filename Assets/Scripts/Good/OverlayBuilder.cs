using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class OverlayBuilder : MonoBehaviour
{

    public GameObject[] BetaObjects;

    public Text BetaName;
    public Text BetaVersion;
    static readonly bool beta = true;
    string versionCache;
    string nameCache;

    public void Start()
    {
        foreach(GameObject go in BetaObjects)
        {
            go.SetActive(beta);
        }

        versionCache = MiscGrabs.grab.Version.strung;
        nameCache = GorillaComputer.instance.offlineVRRigNametagText.text;

        switch (nameCache)
        {
            case "gorilla":
                nameCache = "UNASSIGNED";
            break;
            case "LAPS":
                nameCache = "<color=orange>LAPS</color>";
            break;
            case "SPOOL":
                nameCache = "<color=#1703fc>S</color><color=#4a03fc>P</color><color=#6703fc>O</color><color=#7703fc>O</color><color=#9403fc>L</color>";
            break;
            case "ROASTY":
                nameCache = "<color=#FFD3FF>ROASTY</color>";
            break;
            case "LOST":
                nameCache = "<color=#E2C2FF>LOST</color>";
            break;
        }
    }

    public void Update()
    {
        if(!beta)return;
        
        BetaName.text = "NAME | " + nameCache;
        BetaVersion.text = "REMIX BETA BUILD : ID = " + versionCache;
    }
}
