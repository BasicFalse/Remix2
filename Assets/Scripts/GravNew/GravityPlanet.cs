using UnityEngine;

namespace GravNew
{
    public class GravityPlanet : GravitySectorController
    {
        public Transform planetCenter;

        public override Vector3 CalculateGravityDirection(Vector3 objectPos)
        {
            return planetCenter.position - objectPos;
        }
    }
}