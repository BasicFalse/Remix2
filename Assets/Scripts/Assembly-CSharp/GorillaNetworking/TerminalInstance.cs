using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class TerminalInstance : MonoBehaviour
{
    public string Name;
    public Text Data;
    public Text FuncSelect;
    public Image image;
    public Transform Spwn;

    public void Start()
    {
        GorillaComputer.instance.AddTerminal(this);
    }
}
