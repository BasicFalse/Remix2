using UnityEngine;

namespace GravNew
{
    [RequireComponent(typeof(Collider))]
    public class GravitySector : MonoBehaviour
    {
        public GravSetting gravityType;
        public bool active;
        public bool reorient;
        public float mass;
        public Transform parGravity;
        public Vector3 V3;
        public int priority;

        private void Awake()
        {
            Collider coll = GetComponent<Collider>();
            coll.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GravityPasser>(out var passer) && active)
            {
                passer.EnterSector(this);
            }
            /*
            var grav = other.GetComponent<NormalisedGravity>();
            if (grav != null && active)
            {
                grav.target = parGravity;
                grav.gravityStrength = mass;
                grav.V3 = V3;
                grav.isInSector = true;
                grav.gravType = gravityType;
            }
            */
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<GravityPasser>(out var passer))
            {
                passer.ExitSector(this);
            }
            /*
            var grav = other.GetComponent<NormalisedGravity>();
            if (grav != null && active && grav.target == parGravity)
            {
                grav.target = null;
                grav.isInSector = false;
            }
            */
        }
    }
}
