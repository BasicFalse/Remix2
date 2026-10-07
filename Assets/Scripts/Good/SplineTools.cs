using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineTools{
    public class SplineEvaluation : MonoBehaviour
    {
        public SplineContainer splineContainer;
        [Range(0f, 1f)] public float t; // Position along spline

        /// <summary>
        /// Sets the position of t along the spline based on a float.
        /// </summary>
        /// <param name="newT"></param>
        public virtual void SetT(float newT)
        {
            t = Mathf.Clamp01(newT);
        }

        /// <summary>
        /// Gets the world position of the point t of the spline.
        /// </summary>
        /// <returns>World Position</returns>
        public Vector3 GetWorldPosition()
        {
            return splineContainer.transform.TransformPoint(
                splineContainer.Spline.EvaluatePosition(t)
            );
        }

        /// <summary>
        /// Gets the World pos of a point on a spline.
        /// </summary>
        /// <param name="SC">The Spline Container source.</param>
        /// <param name="t2">The Spline progress to source the position from.</param>
        /// <returns>World Position</returns>
        public static Vector3 GetWorldPosition(SplineContainer SC, float t2)
        {
            return SC.transform.TransformPoint(
                SC.Spline.EvaluatePosition(t2)
            );
        }

        public static Vector3 GetClosestPoint(SplineContainer SC, Vector3 worldPos)
        {
            var spline = SC.Spline;

            Vector3 localPoint = SC.transform.InverseTransformPoint(worldPos);

            Ray ray = new(localPoint + Vector3.up * 100f, Vector3.down);


            SplineUtility.GetNearestPoint(
                spline,
                ray,
                out _,
                out float t2
            );

            return SC.transform.TransformPoint(
                spline.EvaluatePosition(t2)
            );
        }

        public static Vector3 GetForwardTangent(Spline s, float t)
        {
            SplineUtility.Evaluate(
            s,
            t,
            out _,
            out float3 tangent,   // ← this IS the forward tangent
            out _
            );

            // Tangent is already in "forward" direction at t
            // Normalize it if you need a unit direction vector:
            return Vector3.Normalize(tangent);
        }

        public static float GetClosestFloatPoint(SplineContainer s, Vector3 v3)
        {
            Vector3 localPoint = s.transform.InverseTransformPoint(v3);

            Ray ray = new(localPoint + Vector3.up * 100f, Vector3.down);


            SplineUtility.GetNearestPoint(
                s.Spline,
                ray,
                out _,
                out float nearestT
            );

            return nearestT;
        }

    }
}