using UnityEngine;

namespace RemixPortals{
    public class PortalManager : MonoBehaviour
    {
        public PortalPair[] PortalPairs;

        public Material PortalMaterialTemplate;

        void Start()
        {
            for (int i =0; i<PortalPairs.Length; i++)
            {
                PortalPairs[i].SetupPortalPair(PortalMaterialTemplate);
            }
        }
    }
}