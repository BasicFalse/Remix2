using Unity.Cinemachine;
using UnityEngine;

namespace GravNew
{
    public class GravityTube : GravitySectorController
    {
        /// <summary>
        /// The center of the tube is used as, you guessed it, the center of the tube. Gravity will also be projected on the forward of it.
        /// </summary>
        public Transform TubeCenter;
        /// <summary>
        /// If not null, does random shit idek.
        /// </summary>
        public Transform TubeEnd;

        public bool invert = false; 
        
#if UNITY_EDITOR
        private Vector3 cache;
#endif

        public override Vector3 CalculateGravityDirection(Vector3 objectPos)
        {
#if UNITY_EDITOR
            cache = objectPos;
#endif
            Vector3 rawDiff = TubeCenter.position - objectPos;
            if(TubeEnd == null)
            {
                rawDiff = rawDiff.ProjectOntoPlane(TubeCenter.forward);
            }
            else
            {
                rawDiff = rawDiff.ProjectOntoPlane(TubeCenter.position - TubeEnd.position);
            }
            rawDiff = invert ? -rawDiff : rawDiff;
            return rawDiff;
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if(!TubeCenter) return;
            Vector3 rawDiff = TubeCenter.position - cache;
            if(TubeEnd == null)
            {
                rawDiff = rawDiff.ProjectOntoPlane(TubeCenter.forward);
            }
            else
            {
                rawDiff = rawDiff.ProjectOntoPlane(TubeCenter.position - TubeEnd.position);
            }
            rawDiff = invert ? -rawDiff : rawDiff;
            Gizmos.DrawLine(cache, cache + rawDiff);
        }
#endif
    }
}