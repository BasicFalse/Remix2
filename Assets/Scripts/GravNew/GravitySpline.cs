using SplineTools;
using UnityEngine;
using UnityEngine.Splines;

namespace GravNew{
    public class GravitySpline : GravitySectorController
    {
        /// <summary>
        /// Useful for scene controlled gravity (if that makes sense). If null, the objectPos will be used instead.
        /// </summary>
        public Transform Target;

        public bool InvertDirection;

        public SplineContainer SplineOne;
        public SplineContainer SplineTwo;


        public override Vector3 CalculateGravityDirection(Vector3 objectPos)
        {
            if(SplineTwo == null)
            {
                return MonoSplineGravity(objectPos);
            }
            else
            {
                return DualSplineGravity(objectPos);
            }
        }

        private Vector3 MonoSplineGravity(Vector3 objectPos)
        {
            Vector3 Spline1Pos = SplineEvaluation.GetClosestPoint(SplineOne, (Target != null) ? Target.position : objectPos);
            Vector3 direction = (Target != null) ? Target.position : objectPos - Spline1Pos;

            if (InvertDirection){direction = -direction;}

            return direction;
        }

        private Vector3 DualSplineGravity(Vector3 objectPos)
        {
            Vector3 Spline1Pos =
            SplineEvaluation.GetClosestPoint(SplineOne, (Target != null) ? Target.position : objectPos);

            Vector3 Spline2Pos = 
            SplineEvaluation.GetClosestPoint(SplineTwo, Spline1Pos);

            Vector3 direction = Spline1Pos - Spline2Pos;

            if (InvertDirection){direction = -direction;}

            return direction;
        }
    }
}