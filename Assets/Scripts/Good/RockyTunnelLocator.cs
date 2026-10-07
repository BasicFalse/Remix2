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

            TryGetComponent(out MeshRenderer mr);
            if (mr != null && !RTM.renderTriggers) { mr.enabled = false; }
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
