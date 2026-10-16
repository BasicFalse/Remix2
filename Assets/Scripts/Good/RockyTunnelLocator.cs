using System.Collections.Generic;
using UnityEngine;

namespace RockyTunnel {
    public class RockyTunnelLocator : RemixScript
    {
        //public bool trigger;
        public RockyTunnelManager RTM;
        public List<RockyTunnelManager.Split> Designations;

        private void Start()
        {
            if (RTM == null)
            {
                Debug.LogError("There's no RTM on this trigger" + gameObject);
            }

            Collider coll = GetComponent<Collider>();
            TryGetComponent(out MeshRenderer mr1);
            if (mr1 != null && !RTM.renderTriggers) { mr1.enabled = false; }
            
            
            GameObject visible = new();
            visible.transform.parent = gameObject.transform;
            MeshFilter mf = visible.AddComponent<MeshFilter>();
            mf.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
            MeshRenderer mr = visible.AddComponent<MeshRenderer>();
            mr.material = MapGeoController.instance.TriggerMaterial;
            visible.transform.SetPositionAndRotation(transform.position, transform.rotation);
            visible.transform.localScale = new Vector3(1,1,1);
            visible.layer = LayerMask.NameToLayer("DEV");
            visible.name = gameObject.name + ":VISUAL";
            coll.isTrigger = true;
        }

        //[ContextMenu("OnTriggerEnter")]
        private void OnTriggerEnter(Collider other)
        {
            RTM.ChangeCurrentSectors(Designations);
        }
        //private void Update()
        //{
        //    if (trigger)
        //    {
        //        RTM.ChangeCurrentSectors(Designations);
        //        trigger = false;
        //    }
        //}
    }
}
