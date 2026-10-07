using System.Collections.Generic;
using UnityEngine;

namespace RockyTunnel{
    public class RockyTunnelManager : RemixScript
    {
        public enum Split { A1, A2, A3, A4, A5, A6, B1, D3}

        [System.Serializable]
        public class Sector
        {
            [SerializeField]
            public Split Designation;
            public List<Split> NextTo;
            public List<GameObject> Geometry;
        }

        [System.Serializable]
        public class Link
        {
            [SerializeField]
            public List<GameObject> Geometry;
            public List<Split> Links;
        }

        [SerializeField]
        public List<Sector> Sectors;
        public List<Link> Links;
        public List<Split> currentSectors;

        public bool renderTriggers;
        //public List<Sector> lastSectors;

        public override void StartExt()
        {
            Tink();
        }

        [ContextMenu("Tink")]
        private void Tink()
        {
            UpdateSectors();
            UpdateLinks();
        }

        private void UpdateSectors()
        {
            foreach (Sector s in Sectors)
            {
                int a = 0;
                foreach (Split s2 in s.NextTo)
                {
                    if (currentSectors.Contains(s2))
                    {
                        a++;
                    }
                }
                if (currentSectors.Contains(s.Designation) || a > 0)

                { foreach (GameObject g in s.Geometry) { g.SetActive(true); } }

                else

                { foreach (GameObject g in s.Geometry) { g.SetActive(false); } }
            }
        }

        private void UpdateLinks()
        {
            foreach (Link l in Links)
            {
                int a = 0;
                foreach (Split s in l.Links)
                {
                    if (currentSectors.Contains(s))
                    {
                        a++;
                    }
                }
                if (a > 0)
                {
                    foreach (GameObject g in l.Geometry) { g.SetActive(true); }
                }
                else
                {
                    foreach (GameObject g in l.Geometry) { g.SetActive(false); }
                }
            }
        }

        public void ChangeCurrentSectors(List<Split> splits)
        {
            currentSectors = splits;
            Tink();
        }
    }
}


