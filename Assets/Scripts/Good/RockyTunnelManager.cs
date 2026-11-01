using System.Collections.Generic;
using UnityEngine;

namespace RockyTunnel{
    public class RockyTunnelManager : RemixScript
    {
        public enum Split { A1 = 11, A2 = 12, A3 = 13, A4 = 14, A5 = 15, A6 = 16, B1 = 21, D1 = 41, D2 = 42, D3 = 43 }
        public enum TunnelDesignation { A = 1, B = 2, C = 3, D = 4 }

        [System.Serializable]
        public struct Tunnel
        {
            public GameObject Geometry;
            public TunnelDesignation Designation;
        }

        [System.Serializable]
        public struct Sector
        {
            [SerializeField]
            public Split Designation;
            public Split[] NextTo;
            public GameObject[] Geometry;
        }

        [System.Serializable]
        public struct Link
        {
            [SerializeField]
            public GameObject[] Geometry;
            public Split[] Links;
        }

        public Tunnel[] Tunnels;
        public Sector[] Sectors;
        public Link[] Links;
        public List<Split> currentSectors;

        public bool renderTriggers;

        public override void StartExt()
        {
            Tink();
        }

        [ContextMenu("Tink")]
        private void Tink()
        {
            UpdateSectors();
            UpdateLinks();
            UpdateTunnels();
        }

        private void UpdateSectors()
        {
            for (int i = 0; i < Sectors.Length; i++)
            {
                int a = 0;
                for (int j = 0; j < Sectors[i].NextTo.Length; j++)
                {
                    if (currentSectors.Contains(Sectors[i].NextTo[j]))
                    {
                        a++;
                    }
                }
                if (currentSectors.Contains(Sectors[i].Designation) || a > 0)

                { for (int k = 0; k < Sectors[i].Geometry.Length; k++) { Sectors[i].Geometry[k].SetActive(true); } }

                else

                { for (int k = 0; k < Sectors[i].Geometry.Length; k++) { Sectors[i].Geometry[k].SetActive(false); } }
            }
        }

        private void UpdateLinks()
        {
            for (int i = 0; i < Links.Length; i++)
            {
                int a = 0;
                for (int j = 0; j < Links.Length; j++)
                {
                    if (currentSectors.Contains(Links[i].Links[j]))
                    {
                        a++;
                    }
                }
                if (a > 0)
                {
                    for (int k = 0; k < Links[i].Geometry.Length; k++)
                    {
                        Links[i].Geometry[k].SetActive(true);
                    }
                }
                else
                {
                    for (int k = 0; k < Links[i].Geometry.Length; k++)
                    {
                        Links[i].Geometry[k].SetActive(false);
                    }
                }
            }
        }

        private void UpdateTunnels()
        {
            for (int i = 0; i < Tunnels.Length; i++)
            {
                int a = 0;
                for (int j = 0; j < Sectors.Length; j++)
                {
                    if (currentSectors.Contains(Sectors[j].Designation) && (int)Sectors[j].Designation / 10 == (int)Tunnels[i].Designation)
                    {
                        a++;
                    }
                }
                if (a > 0)
                {
                    Tunnels[i].Geometry.SetActive(true);
                }
                else
                {
                    Tunnels[i].Geometry.SetActive(false);
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


