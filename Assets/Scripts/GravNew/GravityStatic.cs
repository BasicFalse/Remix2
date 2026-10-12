using UnityEngine;

namespace GravNew
{
    public class GravityStatic : GravitySectorController
    {
        public Vector3 vector;

        public override Vector3 CalculateGravityDirection(Vector3 objectPos) => vector;
    }
}