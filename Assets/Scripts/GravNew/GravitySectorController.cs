using UnityEngine;

namespace GravNew
{
    public class GravitySectorController : MonoBehaviour
    {
        public bool sendToGravSector;
        public GravSetting gravType;
        public GravitySector[] GravSectors;
        public Transform fromTransform;
        public Transform toTransform;

        public bool ReCalc;
        public bool isRecalculating;

        public bool ignoreX;
        public bool ignoreY;
        public bool ignoreZ;

        public bool reorient;

        public Vector3 filteredDirection;

        void Start()
        {
            if (fromTransform == null || toTransform == null)
            {
                Debug.LogWarning("Transforms not assigned.");
                return;
            }
        }

        public virtual void FixedUpdate()
        {
            if (isRecalculating || ReCalc)
            {
                switch (gravType)
                {
                    case GravSetting.Planet:
                        PlanetBased();
                        break;
                    case GravSetting.Inverted:
                        SectorBased();
                        break;
                    case GravSetting.V3:
                        SectorBased();
                        break;
                    case GravSetting.InvertedV3:
                        SectorBased();
                        break;
                }
                isRecalculating = ReCalc;
            }
        }

        protected void SectorBased()
        {
            Vector3 direction = toTransform.position - fromTransform.position;
            if (ignoreX) direction.x = 0;
            if (ignoreY) direction.y = 0;
            if (ignoreZ) direction.z = 0;
            filteredDirection = direction;
            SendToGravSector();
            
            Debug.DrawRay(fromTransform.position, filteredDirection, Color.green);
        }

        protected void PlanetBased()
        {
            SendToGravSector();
        }

        protected void SendToGravSector()
        {
            if(sendToGravSector){
                for (int i = 0; i < GravSectors.Length; i++){
                    GravSectors[i].gravityType = gravType;
                    GravSectors[i].V3 = filteredDirection;
                    GravSectors[i].reorient = reorient;}
            }
        }
    }
}
