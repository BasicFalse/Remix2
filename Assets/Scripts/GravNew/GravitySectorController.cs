using UnityEngine;

namespace GravNew
{
    public abstract class GravitySectorController : MonoBehaviour
    {
        public GravitySector[] GravSectors;

        public void Awake()
        {
            for (int i = 0; i < GravSectors.Length; i++)
            {
                GravSectors[i].controller = this;
            }
        }

        public abstract Vector3 CalculateGravityDirection(Vector3 objectPos);

    }
}
