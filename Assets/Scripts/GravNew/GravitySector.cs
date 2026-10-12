using UnityEngine;

namespace GravNew
{
    [RequireComponent(typeof(Collider))]
    public class GravitySector : MonoBehaviour
    {
        public GravitySectorController controller;
        public bool active = true;
        public bool reorient = true;
        public float mass = 9.8f;
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
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<GravityPasser>(out var passer))
            {
                passer.ExitSector(this);
            }
        }

        public Vector3 GetGravityDirection(Vector3 Position)
        {
            if (controller)
            {
                return controller.CalculateGravityDirection(Position).normalized;
            }
            else
                return Vector3.one;
        }

        public Vector3 GetGravityVector(Vector3 Position)
        {
            return GetGravityDirection(Position) * mass;
        }
    }
}
