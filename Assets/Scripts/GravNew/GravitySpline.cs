using SplineTools;
using UnityEngine;
using UnityEngine.Splines;

namespace GravNew{
    public class GravitySpline : GravitySectorController
    {
        public Transform Target;

        public GravSplineSetting SplineSetting;
        public bool InvertDirection;

        public SplineContainer SplineOne;
        public SplineContainer SplineTwo;


        public override void FixedUpdate()
        {
            //gravType = GravSetting.V3;
            if(SplineSetting == GravSplineSetting.MonoSpline)
            {
                MonoSplineGravity();
            }
            else
            {
                DualSplineGravity();
            }
        }

        private void MonoSplineGravity()
        {
            Vector3 Spline1Pos = SplineEvaluation.GetClosestPoint(SplineOne, Target.position);
            Vector3 direction = Target.position - Spline1Pos;
            if (InvertDirection){direction = -direction;}

            filteredDirection = direction;

            SendToGravSector();

            Debug.DrawRay(Spline1Pos, filteredDirection, Color.green);
        }

        private void DualSplineGravity()
        {
            Vector3 Spline1Pos =
            SplineEvaluation.GetClosestPoint(SplineOne, Target.position);

            Vector3 Spline2Pos = 
            SplineEvaluation.GetClosestPoint(SplineTwo, Spline1Pos);

            Vector3 direction = Spline1Pos - Spline2Pos;

            if (InvertDirection){direction = -direction;}

            filteredDirection = direction;

            SendToGravSector();
            
            Debug.DrawRay(Spline1Pos, filteredDirection, Color.green);
        }
    }
}