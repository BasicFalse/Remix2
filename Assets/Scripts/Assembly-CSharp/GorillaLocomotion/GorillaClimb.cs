using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
using GorillaLocomotion;
using SplineTools;

namespace GorillaLocomotion{
public class GorillaClimb : MonoBehaviour
{
    public Rigidbody player;
    public PlayerBase locomotion;
    public GorillaVibration vibration;
    public float climbingRange;
    public int ClimbType = 1;
    public LayerMask climbableLayer;
    public AudioSource grabSource;
    public AudioClip defaultGrabSound;
    public AudioClip defaultLetGoSound;
    public EasyHand hand;

    public bool climbing;
    private bool gripHeld;
    private bool pendingGrab;

    public Rigidbody lastHitRb;

    private Vector3 lastHandPos;
    private Vector3 handVelocity;

    private Vector3 playerOffsetLocal;
    private Quaternion initialHandToPlayerRot;

    private Vector3 handToObjLocal;
    private Quaternion handLocalRot;

    private Transform grabbedObject;
    private Zipline grabbedZipline;
    private ClimbableData GatheredData;
    private Vector3 lastObjPos;
    private Quaternion lastObjRot;
    private Quaternion lastObjRotation;

    private Vector3 lastSimHandWS;

    private Vector3 objLinearVel;
    private Vector3 objAngularVel;

    // shit still crashes lol
    public float maxAnglePerFrameDeg = 60f;
    public float maxObjLinearSpeed = 40f;
    public float maxObjAngularSpeedDeg = 2000f;
    public float maxReleaseSpeed = 20f;
    public float maxFrameMove = 3.0f;

    private void Update()
    {
        float grip = EasyInputs.GetGripButtonFloat(hand);
        bool gripNow = grip > 0.1f;
        bool gripDown = gripNow && !gripHeld;
        bool gripUp = !gripNow && gripHeld;

        gripHeld = gripNow;

        bool canClimb = Physics.CheckSphere(transform.position, climbingRange, climbableLayer);

        if (pendingGrab && gripNow && canClimb && !climbing)
        {
            StartClimb();
            pendingGrab = false;
        }

        if (climbing && gripNow)
        {
            if(ClimbType == 1){
                float dt = Time.deltaTime;
                if (dt <= 0f) return;
    
                // 1) Hand drag (real controller movement should move player)
                Vector3 handDelta = transform.position - lastHandPos;
                locomotion.transform.position -= handDelta;
    
                // 2) If we have an attached object, apply object-driven deltas (rotation + translation)
                if (grabbedObject)
                {
                    // --- simulated hand in world-space based on object + stored local offset ---
                    Vector3 simHandWS = grabbedObject.TransformPoint(handToObjLocal);
                    Quaternion simHandRot = grabbedObject.rotation * handLocalRot;
    
                    // --- A) Apply incremental object rotation (clamped) around the simulated hand pivot ---
                    Quaternion incDeltaRot = grabbedObject.rotation * Quaternion.Inverse(lastObjRot);
                    incDeltaRot = NormalizeQuaternionSafe(incDeltaRot);
    
                    // convert to angle-axis & clamp
                    incDeltaRot.ToAngleAxis(out float incAngleDeg, out Vector3 incAxis);
                    if (incAngleDeg > 180f) incAngleDeg -= 360f;
                    if (incAxis == Vector3.zero || float.IsNaN(incAxis.x))
                    {
                        incAngleDeg = 0f;
                        incAxis = Vector3.up;
                    }
    
                    float clampedIncAngle = Mathf.Clamp(incAngleDeg, -maxAnglePerFrameDeg, maxAnglePerFrameDeg);
                    Quaternion safeIncDelta = Quaternion.AngleAxis(clampedIncAngle, incAxis.normalized);
                    safeIncDelta = NormalizeQuaternionSafe(safeIncDelta);
    
                    // rotate player around simulated hand pivot by the INCREMENTAL rotation
                    RotatePlayerAround(simHandWS, safeIncDelta);
    
                    // update lastObjRot immediately (we used incremental)
                    lastObjRot = grabbedObject.rotation;
    
                    // --- B) Apply object translation as delta (so object movement pulls the player) ---
                    Vector3 simHandDelta = simHandWS - lastSimHandWS;
                    if (!IsFiniteVector(simHandDelta)) simHandDelta = Vector3.zero;
                    if (simHandDelta.magnitude > maxFrameMove)
                        simHandDelta = simHandDelta.normalized * maxFrameMove;
    
                    locomotion.transform.position += simHandDelta;
    
                    lastSimHandWS = simHandWS;
    
                    // --- C) Momentum snapshot: linear velocity from object translation ---
                    Vector3 rawObjLinear = simHandDelta / Mathf.Max(dt, 1e-6f);
                    if (!IsFiniteVector(rawObjLinear)) rawObjLinear = Vector3.zero;
                    if (rawObjLinear.magnitude > maxObjLinearSpeed)
                        rawObjLinear = rawObjLinear.normalized * maxObjLinearSpeed;
                    objLinearVel = rawObjLinear;
    
                    // --- D) Angular momentum from incremental rotation ---
                    Quaternion angDelta = grabbedObject.rotation * Quaternion.Inverse(lastObjRotation);
                    angDelta = NormalizeQuaternionSafe(angDelta);
                    angDelta.ToAngleAxis(out float aDeg, out Vector3 aAxis);
                    if (aDeg > 180f) aDeg -= 360f;
    
                    if (aAxis == Vector3.zero || float.IsNaN(aAxis.x))
                    {
                        objAngularVel = Vector3.zero;
                    }
                    else
                    {
                        float degPerSec = aDeg / Mathf.Max(dt, 1e-6f);
                        degPerSec = Mathf.Clamp(degPerSec, -maxObjAngularSpeedDeg, maxObjAngularSpeedDeg);
                        objAngularVel = aAxis.normalized * (degPerSec * Mathf.Deg2Rad);
                    }
    
                    lastObjRotation = grabbedObject.rotation;
    
                    // --- E) angular contribution to linear velocity at grab point ---
                    Vector3 radial = simHandWS - grabbedObject.position;
                    Vector3 angularContribution = Vector3.Cross(objAngularVel, radial);
    
                    Vector3 combined = objLinearVel + angularContribution;
                    if (!IsFiniteVector(combined)) combined = Vector3.zero;
                    if (combined.magnitude > maxObjLinearSpeed)
                        combined = combined.normalized * maxObjLinearSpeed;
    
                    objLinearVel = combined;
    
                    // --- F) update object position baseline for next-frame translation delta
                    lastObjPos = grabbedObject.position;
                }
    
                // 3) compute hand velocity for release
                Vector3 rawHandVel = handDelta / Mathf.Max(Time.deltaTime, 1e-6f);
                if (!IsFiniteVector(rawHandVel)) rawHandVel = Vector3.zero;
                if (rawHandVel.magnitude * locomotion.getSizeRatio > maxReleaseSpeed * 3f)
                    rawHandVel = rawHandVel.normalized * maxReleaseSpeed * 3f * locomotion.getSizeRatio;
                handVelocity = rawHandVel;
                
    
                // 4) update lastHandPos
                lastHandPos = transform.position;
            }

            if(ClimbType == 2)
            {
                
                float dt = Time.deltaTime;
                if (dt <= 0f) return;

                Vector3 handDelta = transform.position - lastHandPos;
                locomotion.transform.position -= handDelta;

                if(grabbedZipline){
                transform.position = SplineEvaluation.GetWorldPosition(grabbedZipline.splineContainer, grabbedZipline.t);
                }

                Vector3 rawHandVel = handDelta / Mathf.Max(Time.deltaTime, 1e-6f);
                if (!IsFiniteVector(rawHandVel)) rawHandVel = Vector3.zero;
                if (rawHandVel.magnitude * locomotion.getSizeRatio > maxReleaseSpeed * 3f)
                    rawHandVel = rawHandVel.normalized * maxReleaseSpeed * 3f * locomotion.getSizeRatio;
                handVelocity = rawHandVel;

                lastHandPos = transform.position;
            }
        }
        

        if (gripDown)
        {
            if (canClimb)
                StartClimb();
            else
                pendingGrab = true;
        }

        if (gripUp)
        {
            pendingGrab = false;
            if (climbing)
                EndClimb();
        }
    }

    private void StartClimb()
    {
        climbing = true;
        player.isKinematic = true;
        locomotion.RPhysical.currentVelocity = Vector3.zero;

        lastHandPos = transform.position;
        handVelocity = Vector3.zero;

        grabbedObject = FindGrabbedObject();
        if (grabbedObject)
        {

            //ClimbType Evaluation
            grabbedObject.TryGetComponent(out ClimbableData component);
            if (component)
            {
                GatheredData = component;
                ClimbType = component.ClimbableType;
            }
            else
            {
                ClimbType = 1;
            }

            if(ClimbType == 1){
                // store player offset in hand-local space (hand.InverseTransformPoint(player))
                playerOffsetLocal = transform.InverseTransformPoint(locomotion.transform.position);

                // store hand -> player rotational offset
                initialHandToPlayerRot = Quaternion.Inverse(transform.rotation) * locomotion.transform.rotation;

                // store hand relative to object (position + rot) at grab time
                handToObjLocal = grabbedObject.InverseTransformPoint(transform.position);
                handLocalRot = Quaternion.Inverse(grabbedObject.rotation) * transform.rotation;

                // baselines
                lastObjPos = grabbedObject.position;
                lastObjRot = grabbedObject.rotation;
                lastObjRotation = grabbedObject.rotation;

                // initialize simulated hand WS baseline
                lastSimHandWS = grabbedObject.TransformPoint(handToObjLocal);

                objLinearVel = Vector3.zero;
                objAngularVel = Vector3.zero;
            }

            if(ClimbType == 2)
            {
                grabbedZipline = component.ZLSc;
                grabbedZipline.SetToClosestPoint(transform.position);
                grabbedZipline.SetVelocityFromRigidbody(player);
            }
        }

        PlayerBase.i.GorillaVibration.StartVibration(
            hand == EasyHand.LeftHand,
            GorillaVibration.tapHapticStrength,
            GorillaVibration.tapHapticDuration
        );

        if (GatheredData.dograbsound)
        {
            grabSource.Stop();
            grabSource.PlayOneShot(GatheredData.grabsound);
            grabSource.mute = true;
        }

        if (GatheredData.doholdsound)
        {
            grabSource.clip = GatheredData.holdsound;
            grabSource.loop = true;
            grabSource.mute = false;
            grabSource.Play();
        }
    }

    private void EndClimb()
    {
        climbing = false;
        player.isKinematic = false;
        Vector3 releaseVel = new();
        if(ClimbType == 1)
        {
            // RELEASE VELOCITY = -handVelocity + object linear contrib (includes angular)
            releaseVel = -handVelocity + objLinearVel;

            if (!IsFiniteVector(releaseVel)) releaseVel = Vector3.zero;
            if (releaseVel.magnitude > maxReleaseSpeed)
                releaseVel = releaseVel.normalized * maxReleaseSpeed;
        }

        if(ClimbType == 2)
        {
            // RELEASE VELOCITY = -handVelocity + object linear contrib (includes angular)
            releaseVel = -handVelocity + grabbedZipline.GetWorldVelocity();

            if (!IsFiniteVector(releaseVel)) releaseVel = Vector3.zero;
            if (releaseVel.magnitude > maxReleaseSpeed)
                releaseVel = releaseVel.normalized * maxReleaseSpeed;
        }
        
        grabbedZipline = null;
        grabbedObject = null;
        player.linearVelocity = releaseVel;

        if (GatheredData.doletgosound)
        {
            grabSource.Stop();
            grabSource.mute = true;
            grabSource.PlayOneShot(GatheredData.letgosound);
        }
        
    }

    private Transform FindGrabbedObject()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, climbingRange, climbableLayer);
        if (hits.Length > 0)
            return hits[0].transform;
        return null;
    }

    private void RotatePlayerAround(Vector3 pivot, Quaternion deltaRot)
    {
        deltaRot = NormalizeQuaternionSafe(deltaRot);

        Transform p = locomotion.transform;

        Vector3 dir = p.position - pivot;
        Vector3 newDir = deltaRot * dir;

        Vector3 dirDelta = newDir - dir;
        if (!IsFiniteVector(dirDelta)) dirDelta = Vector3.zero;
        if (dirDelta.magnitude > maxFrameMove)
            newDir = dir + dirDelta.normalized * maxFrameMove;

        p.position = pivot + newDir;
        p.rotation = deltaRot * p.rotation;
    }

    private static Quaternion NormalizeQuaternionSafe(Quaternion q)
    {
        if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w))
            return Quaternion.identity;
        float mag = Mathf.Sqrt(q.x*q.x + q.y*q.y + q.z*q.z + q.w*q.w);
        if (mag < 1e-6f) return Quaternion.identity;
        return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }

    private static bool IsFiniteVector(Vector3 v)
    {
        return !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z)
                 || float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z));
    }
}
}