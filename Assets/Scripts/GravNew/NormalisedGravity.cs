using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GravNew
{
    [RequireComponent(typeof(Rigidbody))]
    public class NormalisedGravity : MonoBehaviour
    {
        public static volatile NormalisedGravity instance;
        public bool useV3;
        public GravSetting gravType;
        public Vector3 V3;
        public Vector3 Rot3;
        public Transform target;
        public Transform rotTarget;
        public float gravityStrength = 20f;
        public float uprightTorque = 5f;
        public Vector3 forwardReference = Vector3.forward;
        public Vector3 CurrentGravityVelocity => Vector3.Project(rb.linearVelocity, gravityDirection);
        public Vector3 gravityVelocity => Vector3.Project(rb.linearVelocity, gravityDirection);
        public Vector3 gravityDirection;
        public bool isInSector;
        public bool negate;
        public bool isPlayer;
        public bool zeroG;
        public VRSwimming swimmer;
        public PlayerBase locomotion;
        public Rigidbody rb;
        public bool reorient;
        public float rbDragLock;
        public float rbDragUnlock;

        void Start()
        {
            if (isPlayer)
            {
                if (instance == null) { instance = this; }
                else if (instance != this) { UnityEngine.Object.Destroy(base.gameObject); }
            }
            if (rb == null) { rb = GetComponent<Rigidbody>(); }
        }

        public void UpdateAlignment(bool instant = false)
        {
            switch (gravType)
            {
                case GravSetting.Planet:
                    useV3 = false;
                    negate = false;
                    break;
                case GravSetting.Inverted:
                    useV3 = false;
                    negate = true;
                    break;
                case GravSetting.V3:
                    useV3 = true;
                    negate = false;
                    break;
                case GravSetting.InvertedV3:
                    useV3 = true;
                    negate = true;
                    break;
            }

            if (instant)
            {
                Vector3 rotCenter = useV3 ? Rot3.normalized : (rotTarget.position - transform.position).normalized;
                Vector3 rotDir = negate ? -rotCenter : rotCenter;

                Vector3 RotUp = -rotDir;

                Vector3 localForward = transform.forward;
                Vector3 surfaceForward = Vector3.ProjectOnPlane(localForward, RotUp).normalized;

                if (surfaceForward.sqrMagnitude < 0.01f)
                {
                    surfaceForward = Vector3.ProjectOnPlane(forwardReference, RotUp).normalized;
                }

                rb.rotation = Quaternion.LookRotation(surfaceForward, RotUp);
            }
            else
            {
                rb.linearDamping = rbDragLock;

                Vector3 rotCenter = useV3 ? Rot3.normalized : (rotTarget.position - transform.position).normalized;
                Vector3 rotDir = negate ? -rotCenter : rotCenter;

                Vector3 RotUp = -rotDir;

                Vector3 localForward = transform.forward;
                Vector3 surfaceForward = Vector3.ProjectOnPlane(localForward, RotUp).normalized;

                if (surfaceForward.sqrMagnitude < 0.01f)
                {
                    surfaceForward = Vector3.ProjectOnPlane(forwardReference, RotUp).normalized;
                }

                if (reorient)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(surfaceForward, RotUp);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * uprightTorque));
                }
            }
        }

        public void UpdateGravity()
        {
            switch (gravType)
            {
                case GravSetting.Planet:
                    useV3 = false;
                    negate = false;
                    break;
                case GravSetting.Inverted:
                    useV3 = false;
                    negate = true;
                    break;
                case GravSetting.V3:
                    useV3 = true;
                    negate = false;
                    break;
                case GravSetting.InvertedV3:
                    useV3 = true;
                    negate = true;
                    break;
            }

            rb.useGravity = false;

            if (isInSector)
            {
                rb.linearDamping = rbDragLock;

                // Determine gravity direction
                Vector3 toCenter = useV3 ? V3.normalized : (target.position - transform.position).normalized;
                Vector3 rotCenter = useV3 ? Rot3.normalized : (rotTarget.position - transform.position).normalized;
                gravityDirection = negate ? -toCenter : toCenter;
                Vector3 rotDir = negate ? -rotCenter : rotCenter;

                if(!swimmer.bodyWater)
                rb.AddForce(gravityDirection * gravityStrength * locomotion.getSizeRatio, ForceMode.Acceleration);
                else
                rb.AddForce(gravityDirection * gravityStrength/2 * locomotion.getSizeRatio, ForceMode.Acceleration);

                Vector3 gravityUp = -gravityDirection;
                Vector3 RotUp = -rotDir;

                Vector3 localForward = transform.forward;
                Vector3 surfaceForward = Vector3.ProjectOnPlane(localForward, RotUp).normalized;

                if (surfaceForward.sqrMagnitude < 0.01f)
                {
                    surfaceForward = Vector3.ProjectOnPlane(forwardReference, RotUp).normalized;
                }

                if (reorient)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(surfaceForward, RotUp);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * uprightTorque));
                }
            }
            else
            {
                rb.linearDamping = rbDragUnlock;

                gravityDirection = Vector3.down;

                if (!zeroG)
                {
                    if(!swimmer.bodyWater.InWater)
                    rb.AddForce(Vector3.down * 9f * locomotion.getSizeRatio, ForceMode.Acceleration);
                    else
                    rb.AddForce(Vector3.down * 4.5f * locomotion.getSizeRatio, ForceMode.Acceleration);

                    // Rotate upright in world space (Y+ up, Z+ forward projected onto XZ plane)
                    Vector3 worldUp = Vector3.up;
                    Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, worldUp).normalized;

                    if (flatForward.sqrMagnitude < 0.01f)
                        flatForward = Vector3.forward;

                    if (reorient)
                    {
                        Quaternion uprightRotation = Quaternion.LookRotation(flatForward, worldUp);
                        rb.MoveRotation(Quaternion.Slerp(rb.rotation, uprightRotation, Time.fixedDeltaTime * uprightTorque));
                    }
                    
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (target == null) return;

            Gizmos.color = Color.red;
            Vector3 origin = transform.position;
            Vector3 toTarget = (target.position - origin).normalized;

            Gizmos.DrawLine(origin, origin + toTarget);
            Gizmos.DrawSphere(origin + toTarget, 0.1f);
        }
    }
}