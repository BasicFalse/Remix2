using System.Collections.Generic;
using UFunction;
using UnityEngine;

public class BEAMLocal : MonoBehaviour
{
    public string BEAMState;
    public bool isOn;
    public MeshRenderer lights;
    public QueryCatalog Query;
    public static List<BEAMLocal> allBeams = new List<BEAMLocal>();

    private void Start()
    {
        if (lights == null)
        {
            lights = GetComponent<MeshRenderer>();
            if (lights == null)
            {
                Debug.LogWarning($"BEAMLocal on {gameObject.name} has no MeshRenderer assigned or found.");
                return;
            }
        }

        if (BEAMManager.instance == null)
        {
            Debug.LogError("BEAMManager.instance is null! Make sure BEAMManager is initialized.");
            return;
        }

        if (Query == null)
        {
            Debug.LogError("Query Catalog is not present on this BEAM", this);
        }

        //lights.material = isOn ? BEAMManager.instance.BEAMOnMaterial : BEAMManager.instance.BEAMOffMaterial;

        ReaddToList();
    }

    public void UpdateLights(bool ActiveState, Material OnMat = null, Material OffMat = null)
    {
        isOn = ActiveState;

        if(OnMat == null){ OnMat = Query.BEAMONMATERIAL; }
        if(OffMat == null){ OffMat = Query.BEAMOFFMATERIAL; }

        if (lights == null)
        {
            Debug.LogWarning("MeshRenderer (lights) is not assigned.");
            return;
        }

        lights.material = isOn ? OnMat : OffMat;
    }

    public void UpdateState(string newState)
    {
        BEAMState = newState;
    }

    public void ReaddToList()
    {
        allBeams ??= new List<BEAMLocal>();

        if (!allBeams.Contains(this))
        {
            allBeams.Add(this);
        }
    }
}
