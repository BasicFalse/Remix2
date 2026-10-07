using UnityEngine.UI;
using GorillaNetworking;

public class MOTDBoard : RemixScript
{
    private const string fetch = "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/MOTD.txt";

    public Text text;

    public void Start()
    {
        text.text = GorillaComputer.instance.FetchMODTString(fetch);
    }
}
