using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MapTrigger : MonoBehaviour
{
    public bool trigger;
    [System.Serializable]
    public enum MapFunction { Add, Remove, Reset, Null }
    [System.Serializable]
    public class MapTriggerSegment
    {
        [SerializeField]
        public string mapName;

        public MapFunction Function;
    }

    public List<MapTriggerSegment> maps;

    public Collider coll;
    public MapGeoController MGC;

    public void Awake()
    {
        if(MGC == null)
        {
            Debug.LogError(gameObject.name + "'s MGC adresss is null", this.gameObject);
            return;
        }
        if(maps == null)
        {
            Debug.LogError(gameObject.name + "'s MapTrigger selection is null", this.gameObject);
            return;
        }

        GetComponent<MeshRenderer>().enabled = !MGC.removeTriggerRenderers;

        GameObject visible = new();
        visible.transform.parent = gameObject.transform;
        MeshFilter mf = visible.AddComponent<MeshFilter>();
        mf.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer mr = visible.AddComponent<MeshRenderer>();
        mr.material = MGC.TriggerMaterial;
        visible.transform.SetPositionAndRotation(transform.position, transform.rotation);
        visible.transform.localScale = new Vector3(1,1,1);
        visible.layer = LayerMask.NameToLayer("DEV");
        visible.name = gameObject.name + ":VISUAL";
        coll.isTrigger = true;
    }

    private void Update()
    {
        if (trigger) { MGC.ModifyCurrentMaps(maps);trigger = false; }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        MGC.ModifyCurrentMaps(maps);
    }
}
