using UnityEngine;

public class BEAMManager : MonoBehaviour
{
    public static volatile BEAMManager instance;

    public Material BEAMOnMaterial;
    public Material BEAMOffMaterial;

    public void Awake()
    {
        if (instance == null) { instance = this; }
        else
        if (instance != this) { Destroy(base.gameObject); }
    }
}
