using System.Collections.Generic;
using easyInputs;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

public class PlayerPhysical : PlayerMovementStyle
{
    public bool disableMovement;
    public string grabbingObject;
    public SphereCollider headCollider;
    public Transform leftHandFollower;
    public Transform rightHandFollower;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public GorillaClimb LHGC;
    public GorillaClimb RHGC;
    private Vector3 lastLeftHandPosition;
    private Vector3 lastRightHandPosition;
    public Vector3 lastHeadPosition;
    private Rigidbody playerRigidBody;
    public MoveSpaceDetector MSDetect;
    public int velocityHistorySize;
    public float maxArmLength = 1f;
    public float unStickDistance = 1f;
    public float velocityLimit;
    public float slideVelocityLimit;
    public float maxJumpSpeed;
    public float jumpMultiplier;
    public float minimumRaycastDistance = 0.05f;
    public float defaultSlideFactor = 0.03f;
    public float slidingMinimum = 0.9f;
    public const float defaultPrecision = 0.99f;
    public float sizeRatio = 1f;
    private const float slideControl = 0.0125f;
    public float stickDepth = 0.01f;
    public Vector3[] velocityHistory;
    private int velocityIndex;
    public Vector3 currentVelocity;
    private Vector3 denormalizedVelocityAverage;
    private Vector3 lastPosition;
    public Vector3 rightHandOffset;
    public Vector3 leftHandOffset;
    public Vector3 bodyOffset;
    
    public LayerMask locomotionEnabledLayers;
    public bool wasLeftHandTouching;
    public bool wasRightHandTouching;
    public bool wasHeadTouching;
    public bool wasBodyTouching;
    public int currentMaterialIndex;
    public bool leftHandSlide;
    public Vector3 leftHandSlideNormal;
    public bool rightHandSlide;
    public Vector3 rightHandSlideNormal;
    public bool headSlide;
    public Vector3 headSlideNormal;
    public float rightHandSlipPercentage;
    public float leftHandSlipPercentage;
    public float headSlipPercentage;
    public bool wasLeftHandSlide;
    public bool wasRightHandSlide;
    public bool wasHeadSlide;
    public bool inOverlay;
    public bool didATurn;
    public GameObject turnParent;
    public int leftHandMaterialTouchIndex;
    public GorillaSurfaceOverride leftHandSurfaceOverride;
    public int rightHandMaterialTouchIndex;
    public GorillaSurfaceOverride rightHandSurfaceOverride;
    private GorillaSurfaceOverride currentOverride;
    public Rigidbody LeftHandHitRigidbody;
    public Rigidbody RightHandHitRigidbody;
    private bool leftHandColliding;
    private bool rightHandColliding;
    private bool headColliding;
    private Vector3 finalPosition;
    private Vector3 rigidBodyMovement;
    private Vector3 firstIterationLeftHand;
    private Vector3 firstIterationRightHand;
    private Vector3 firstIterationHead;
    private RaycastHit hitInfo;
    private float slipPercentage;
    private Vector3 distanceTraveled;
    private Vector3 movementToProjectedAboveCollisionPlane;
    private MeshCollider meshCollider;
    private Mesh collidedMesh;
    private MaterialData foundMatData;
    private string findMatName;
    private int vertex1;
    private int vertex2;
    private int vertex3;
    private List<int> trianglesList = new List<int>(1000000);
    private List<Material> materialsList = new List<Material>(50);
    private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>();
    private int[] sharedMeshTris;
    private float lastRealTime;
    private float calcDeltaTime;
    private float tempRealTime;
    private Vector3 slideAverage;
    private Vector3 slideAverageNormal;
    public RaycastHit tempHitInfo;
    private RaycastHit junkHit;
    private Vector3 firstPosition;
    public RaycastHit tempIterativeHit;
    private float maxSphereSize1;
    private float maxSphereSize2;
    private Collider[] overlapColliders = new Collider[10];
    private int overlapAttempts;
    private int touchPoints;
    private float averageSlipPercentage;
    private Vector3 surfaceDirection;
    public bool LGrabbing;
    public bool RGrabbing;

    public void Start()
    {
        InitializeValues();
    }

    public void InitializeValues()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        velocityHistory = new Vector3[velocityHistorySize];
        for (int i = 0; i < velocityHistory.Length; i++)
        {
            velocityHistory[i] = Vector3.zero;
        }
        lastLeftHandPosition = leftHandFollower.transform.position;
        lastRightHandPosition = rightHandFollower.transform.position;
        lastHeadPosition = headCollider.transform.position;
        velocityIndex = 0;
        denormalizedVelocityAverage = Vector3.zero;
        lastPosition = base.transform.position;
        lastRealTime = Time.realtimeSinceStartup;
        //leftHandVisualOffset = leftHandFollower.transform.position - leftHandRotFix.position;
        //rightHandVisualOffset = rightHandFollower.transform.position - rightHandRotFix.position;
    }

    private Vector3 CurrentLeftHandPosition()
    {
        if (inOverlay)
        {
            return headCollider.transform.position + headCollider.transform.up * -0.5f;
        }
        //if (IsTouchingRope(leftHandTransform, leftHandOffset, out LRS))
        if ((PositionWithOffset(leftHandTransform, leftHandOffset) - headCollider.transform.position).magnitude < maxArmLength*sizeRatio)
        {
            return PositionWithOffset(leftHandTransform, leftHandOffset);
        }
        return headCollider.transform.position + (PositionWithOffset(leftHandTransform, leftHandOffset) - headCollider.transform.position).normalized * maxArmLength*sizeRatio;
    }
    private Vector3 CurrentRightHandPosition()
    {
        if (inOverlay)
        {
            return headCollider.transform.position + headCollider.transform.up * -0.5f;
        }
        //if(IsTouchingRope(rightHandTransform, rightHandOffset, out RRS))
        if ((PositionWithOffset(rightHandTransform, rightHandOffset) - headCollider.transform.position).magnitude < maxArmLength*sizeRatio)
        {
            return PositionWithOffset(rightHandTransform, rightHandOffset);
        }
        return headCollider.transform.position + (PositionWithOffset(rightHandTransform, rightHandOffset) - headCollider.transform.position).normalized * maxArmLength*sizeRatio;
    }

    public bool IsHandTouching(bool forLeftHand)
    {
        if (forLeftHand)
        {
            return wasLeftHandTouching;
        }
        return wasRightHandTouching;
    }

    private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
    {
        return transformToModify.position + transformToModify.rotation * offsetVector;
    }

    public override void UpdatePlayer()
    {
        leftHandColliding = false;
        rightHandColliding = false;
        headColliding = false;
        leftHandSlide = false;
        rightHandSlide = false;
        headSlide = false;
        Vector3 center = @b.colliders.bodyCollider.transform.position + @b.colliders.bodyCollider.center;
        float halfHeight = Mathf.Max(@b.colliders.bodyCollider.height / 2f - @b.colliders.bodyCollider.radius, 0f);
        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center - Vector3.up * halfHeight;
        bool isBodyTouching = Physics.OverlapCapsule(point1, point2, @b.colliders.bodyCollider.radius, locomotionEnabledLayers.value).Length > 0;
        wasBodyTouching = isBodyTouching;
        
        rigidBodyMovement = Vector3.zero;
        firstIterationLeftHand = Vector3.zero;
        firstIterationRightHand = Vector3.zero;
        firstIterationHead = Vector3.zero;
        rightHandSlideNormal = Vector3.up;
        leftHandSlideNormal = Vector3.up;
        headSlideNormal = Vector3.up;
        tempRealTime = Time.realtimeSinceStartup;
        calcDeltaTime = tempRealTime - lastRealTime;
        lastRealTime = tempRealTime;
        sizeRatio = @b.transforms.PlayerRoot.localScale.x;
        Vector3 moveSpaceDelta = GetMoveSpacePositionDelta();
        //leftHandRotFix.position = leftHandFollower.position + (leftHandVisualOffset * sizeRatio);
        //rightHandRotFix.position = rightHandFollower.position + (rightHandVisualOffset * sizeRatio);
        if (calcDeltaTime > 0.1f)
        {
            calcDeltaTime = 0.05f;
        }
        if (EasyInputs.GetGripButtonFloat(EasyHand.LeftHand) >= 0.8)
        {
            LGrabbing = true;
        }
        else { LGrabbing = false; }
        if (EasyInputs.GetGripButtonFloat(EasyHand.RightHand) >= 0.8)
        {
            RGrabbing = true;
        }
        else { RGrabbing = false; }
        if (wasLeftHandSlide || wasRightHandSlide || wasHeadSlide)
        {
            if (averageSlipPercentage > slidingMinimum)
            {
                base.transform.position = base.transform.position + slideAverage * calcDeltaTime + 4.9f * calcDeltaTime * calcDeltaTime * Vector3.down;
                slideAverage += 9.8f * calcDeltaTime * Vector3.down;
            }
            else
            {
                base.transform.position = base.transform.position + slideAverage * calcDeltaTime + 0.6f * calcDeltaTime * calcDeltaTime * Vector3.down;
                slideAverage += 1f * calcDeltaTime * Vector3.down;
            }
            playerRigidBody.linearVelocity = slideAverage * sizeRatio;
        }
        else
        {
            slideAverage = Vector3.zero;
        }
        distanceTraveled = CurrentLeftHandPosition() - lastLeftHandPosition;
        if (IterativeCollisionSphereCast(lastLeftHandPosition, minimumRaycastDistance, distanceTraveled, out finalPosition, singleHand: true, out slipPercentage, out tempHitInfo, out LeftHandHitRigidbody))
        {
            if (wasLeftHandTouching && slipPercentage <= defaultSlideFactor)
            {
                firstIterationLeftHand = lastLeftHandPosition - CurrentLeftHandPosition();
            }
            else
            {
                firstIterationLeftHand = finalPosition - CurrentLeftHandPosition();
            }
            leftHandSlipPercentage = slipPercentage;
            leftHandSlide = slipPercentage > defaultSlideFactor;
            leftHandSlideNormal = tempHitInfo.normal;
            if (Vector3.Dot(denormalizedVelocityAverage, leftHandSlideNormal) <= 0f)
            {
                firstIterationLeftHand = firstIterationLeftHand - Mathf.Min(stickDepth, Vector3.Project(denormalizedVelocityAverage, leftHandSlideNormal).magnitude * calcDeltaTime) * leftHandSlideNormal + Vector3.down * 0.5f * 9.8f * calcDeltaTime * calcDeltaTime;
            }
            else
            {
                firstIterationLeftHand += Vector3.down * 0.5f * 9.8f * calcDeltaTime * calcDeltaTime;
            }
            leftHandColliding = true;
            leftHandMaterialTouchIndex = currentMaterialIndex;
            leftHandSurfaceOverride = currentOverride;
        }
        distanceTraveled = CurrentRightHandPosition() - lastRightHandPosition;
        if (IterativeCollisionSphereCast(lastRightHandPosition, minimumRaycastDistance, distanceTraveled, out finalPosition, singleHand: true, out slipPercentage, out tempHitInfo, out RightHandHitRigidbody))
        {
            if (wasRightHandTouching && slipPercentage <= defaultSlideFactor)
            {
                firstIterationRightHand = lastRightHandPosition - CurrentRightHandPosition();
            }
            else
            {
                firstIterationRightHand = finalPosition - CurrentRightHandPosition();
            }
            rightHandSlipPercentage = slipPercentage;
            rightHandSlide = slipPercentage > defaultSlideFactor;
            rightHandSlideNormal = tempHitInfo.normal;
            if (Vector3.Dot(denormalizedVelocityAverage, rightHandSlideNormal) <= 0f)
            {
                firstIterationRightHand = firstIterationRightHand - Mathf.Min(stickDepth, Vector3.Project(denormalizedVelocityAverage, rightHandSlideNormal).magnitude * calcDeltaTime) * rightHandSlideNormal + Vector3.down * 0.5f * 9.8f * calcDeltaTime * calcDeltaTime;
            }
            else
            {
                firstIterationRightHand += Vector3.down * 0.5f * 9.8f * calcDeltaTime * calcDeltaTime;
            }
            rightHandColliding = true;
            rightHandMaterialTouchIndex = currentMaterialIndex;
            rightHandSurfaceOverride = currentOverride;
        }
        distanceTraveled = headCollider.transform.position - lastHeadPosition;
        if (IterativeCollisionSphereCast(lastHeadPosition, headCollider.radius, distanceTraveled, out finalPosition, singleHand: true, out slipPercentage, out tempHitInfo, out _))
        {
            firstIterationHead = finalPosition - headCollider.transform.position;
            headSlipPercentage = slipPercentage;
            headSlide = slipPercentage > defaultSlideFactor;
            headSlideNormal = tempHitInfo.normal;
            headColliding = true;
        }
        Debug.DrawLine(headCollider.transform.position, headCollider.transform.position + firstIterationHead, Color.green);
        touchPoints = 0;
        rigidBodyMovement = Vector3.zero;
        if (leftHandColliding || wasLeftHandTouching)
        {
            if(LeftHandHitRigidbody){
                rigidBodyMovement += firstIterationLeftHand/2;
                LeftHandHitRigidbody.AddForceAtPosition(-firstIterationLeftHand*playerRigidBody.mass/2, CurrentLeftHandPosition(), ForceMode.VelocityChange);
            }
            else
            {
                rigidBodyMovement += firstIterationLeftHand;
            }
            touchPoints++;
        }
        if (rightHandColliding || wasRightHandTouching)
        {
            if(RightHandHitRigidbody){
                rigidBodyMovement += firstIterationRightHand/2;
                RightHandHitRigidbody.AddForceAtPosition(-firstIterationRightHand*playerRigidBody.mass/2, CurrentRightHandPosition(), ForceMode.VelocityChange);
            }
            else
            {
                rigidBodyMovement += firstIterationRightHand;
            }
            touchPoints++;
        }
        if (headColliding || wasHeadTouching)
        {
            rigidBodyMovement += firstIterationHead;
            touchPoints++;
        }
        if (touchPoints != 0)
            rigidBodyMovement /= (float)touchPoints;
        

        if(wasLeftHandTouching || wasRightHandTouching || wasHeadTouching || wasBodyTouching || isBodyTouching)
            rigidBodyMovement += moveSpaceDelta;
        
        if (IterativeCollisionSphereCast(lastHeadPosition, headCollider.radius, headCollider.transform.position + rigidBodyMovement - lastHeadPosition, out finalPosition, singleHand: false, out slipPercentage, out junkHit, out _))
        {
            rigidBodyMovement = finalPosition - headCollider.transform.position;
            headSlipPercentage = slipPercentage;
            headSlide = slipPercentage > defaultSlideFactor;
            headSlideNormal = junkHit.normal;
            if (Physics.Raycast(lastHeadPosition, headCollider.transform.position - lastHeadPosition + rigidBodyMovement, out hitInfo, (headCollider.transform.position - lastHeadPosition + rigidBodyMovement).magnitude, locomotionEnabledLayers.value))
            {
                rigidBodyMovement = lastHeadPosition - headCollider.transform.position;
                headSlide = false;
            }
        }

        if (rigidBodyMovement != Vector3.zero)
        {
            if(!LHGC.climbing && !RHGC.climbing)
            {
                playerRigidBody.transform.position = playerRigidBody.position + rigidBodyMovement;
            }
        }

        lastHeadPosition = headCollider.transform.position;
        distanceTraveled = CurrentLeftHandPosition() - lastLeftHandPosition;
        lastLeftHandPosition += moveSpaceDelta;
        lastRightHandPosition += moveSpaceDelta;
        lastHeadPosition += moveSpaceDelta;
        if (IterativeCollisionSphereCast(lastLeftHandPosition, minimumRaycastDistance, distanceTraveled, out finalPosition, (!leftHandColliding && !wasLeftHandTouching) || (!rightHandColliding && !wasRightHandTouching), out slipPercentage, out junkHit, out LeftHandHitRigidbody))
        {
            Debug.DrawLine(finalPosition, lastLeftHandPosition, Color.yellow);
            lastLeftHandPosition = finalPosition;
            leftHandColliding = true;
            leftHandMaterialTouchIndex = currentMaterialIndex;
            leftHandSurfaceOverride = currentOverride;
        }
        else
        {
            lastLeftHandPosition = CurrentLeftHandPosition();
        }
        distanceTraveled = CurrentRightHandPosition() - lastRightHandPosition;
        if (IterativeCollisionSphereCast(lastRightHandPosition, minimumRaycastDistance, distanceTraveled, out finalPosition, (!leftHandColliding && !wasLeftHandTouching) || (!rightHandColliding && !wasRightHandTouching), out slipPercentage, out junkHit, out RightHandHitRigidbody))
        {
            lastRightHandPosition = finalPosition;
            rightHandColliding = true;
            rightHandMaterialTouchIndex = currentMaterialIndex;
            rightHandSurfaceOverride = currentOverride;
        }
        else
        {
            lastRightHandPosition = CurrentRightHandPosition();
        }
        StoreVelocities();
        if (rightHandSlide || leftHandSlide || headSlide)
        {
            slideAverageNormal = Vector3.zero;
            touchPoints = 0;
            averageSlipPercentage = 0f;
            if (headSlide)
            {
                slideAverageNormal += headSlideNormal.normalized;
                averageSlipPercentage += headSlipPercentage;
                touchPoints++;
            }
            if (leftHandSlide)
            {
                slideAverageNormal += leftHandSlideNormal.normalized;
                averageSlipPercentage += leftHandSlipPercentage;
                touchPoints++;
            }
            if (rightHandSlide)
            {
                slideAverageNormal += rightHandSlideNormal.normalized;
                averageSlipPercentage += rightHandSlipPercentage;
                touchPoints++;
            }
            slideAverageNormal = slideAverageNormal.normalized;
            averageSlipPercentage = (float)touchPoints / averageSlipPercentage;
            if (!wasLeftHandSlide && !wasRightHandSlide && !wasHeadSlide)
            {
                slideAverage = playerRigidBody.linearVelocity;
            }
            else
            {
                slideAverage = ((Vector3.Dot(denormalizedVelocityAverage, slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(slideAverage, slideAverageNormal) : slideAverage);
            }
            if (touchPoints == 1 && !headSlide)
            {
                surfaceDirection = (rightHandSlide ? Vector3.ProjectOnPlane(rightHandTransform.forward, rightHandSlideNormal) : Vector3.ProjectOnPlane(leftHandTransform.forward, leftHandSlideNormal));
                if (Vector3.Dot(slideAverage, surfaceDirection) > 0f)
                {
                    slideAverage = Vector3.Slerp(slideAverage, surfaceDirection.normalized * slideAverage.magnitude, slideControl);
                }
                else
                {
                    slideAverage = Vector3.Slerp(slideAverage, -surfaceDirection.normalized * slideAverage.magnitude, slideControl);
                }
            }
            playerRigidBody.linearVelocity = Vector3.zero;//@b.gravity.CurrentGravityVelocity;
        }
        else if (leftHandColliding || rightHandColliding)
        {
            playerRigidBody.linearVelocity = Vector3.zero;//@b.gravity.CurrentGravityVelocity;
        }
        else if (wasLeftHandSlide || wasRightHandSlide || wasHeadSlide)
        {
            playerRigidBody.linearVelocity = denormalizedVelocityAverage*sizeRatio;//+@b.gravity.CurrentGravityVelocity;
        }
        if ((rightHandColliding || leftHandColliding) && !disableMovement && !didATurn && !LHGC.climbing && !RHGC.climbing)
        {
            if (rightHandSlide || leftHandSlide || headSlide)
            {
                if (Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude > slideVelocityLimit && Vector3.Dot(denormalizedVelocityAverage, slideAverageNormal) > 0f)
                {
                    //Debug.Log("did a sliding jump");
                    slideAverage = Mathf.Min(maxJumpSpeed * sizeRatio, Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude) * slideAverageNormal.normalized + slideAverage;
                    //if(HitRigidbody) {playerRigidBody.linearVelocity += HitRigidbody.linearVelocity;}
                    //if(HitRigidbody) {playerRigidBody.angularVelocity += HitRigidbody.angularVelocity;}
                }
            }
            else if (denormalizedVelocityAverage.magnitude > velocityLimit)
            {
                //Debug.Log("did a regular jump");
                playerRigidBody.linearVelocity = Mathf.Min(maxJumpSpeed * sizeRatio, jumpMultiplier * denormalizedVelocityAverage.magnitude) * denormalizedVelocityAverage.normalized;
                //if(HitRigidbody) {playerRigidBody.linearVelocity += HitRigidbody.linearVelocity;}
                //if(HitRigidbody) {playerRigidBody.angularVelocity += HitRigidbody.angularVelocity;}
            }
        }
        if (leftHandColliding && (CurrentLeftHandPosition() - lastLeftHandPosition).magnitude > unStickDistance && !Physics.SphereCast(headCollider.transform.position, minimumRaycastDistance * defaultPrecision, CurrentLeftHandPosition() - headCollider.transform.position, out hitInfo, (CurrentLeftHandPosition() - headCollider.transform.position).magnitude - minimumRaycastDistance, locomotionEnabledLayers.value))
        {
            lastLeftHandPosition = CurrentLeftHandPosition();
            leftHandColliding = false;
        }
        if (rightHandColliding && (CurrentRightHandPosition() - lastRightHandPosition).magnitude > unStickDistance && !Physics.SphereCast(headCollider.transform.position, minimumRaycastDistance * defaultPrecision, CurrentRightHandPosition() - headCollider.transform.position, out hitInfo, (CurrentRightHandPosition() - headCollider.transform.position).magnitude - minimumRaycastDistance, locomotionEnabledLayers.value))
        {
            lastRightHandPosition = CurrentRightHandPosition();
            rightHandColliding = false;
        }
        //Quaternion rhq = Quaternion.Inverse(transform.rotation) * rightHandTransform.localRotation;
        //Quaternion lhq = Quaternion.Inverse(transform.rotation) * leftHandTransform.localRotation;
        leftHandFollower.position = lastLeftHandPosition;
        leftHandFollower.rotation = leftHandTransform.parent.rotation * leftHandTransform.localRotation; //leftHandFollower.rotation * Quaternion.Inverse(transform.rotation) * leftHandTransform.rotation;//leftHandTransform.localRotation;//UndoThenAdd(transform.rotation,leftHandTransform.localRotation);////UndoThenAdd(transform.rotation,lhq);
        rightHandFollower.position = lastRightHandPosition;
        rightHandFollower.rotation = rightHandTransform.parent.rotation * rightHandTransform.localRotation;//UndoThenAdd(transform.rotation,rightHandTransform.localRotation);////UndoThenAdd(transform.rotation,rhq);
        wasLeftHandTouching = leftHandColliding;
        wasRightHandTouching = rightHandColliding;
        wasLeftHandSlide = leftHandSlide;
        wasRightHandSlide = rightHandSlide;
        wasHeadSlide = headSlide;
        //if(HitRigidbody) {playerRigidBody.linearVelocity += HitRigidbody.linearVelocity;
        //    playerRigidBody.angularVelocity += HitRigidbody.angularVelocity;
        //    playerRigidBody.position += HitRigidbody.transform.position - HitRBLastPos;
        //    HitRBLastPos = HitRigidbody.transform.position;
        //}
        if (foundMatData.canGrab)
        {
            if (wasLeftHandTouching)
            {
                if (LGrabbing)
                {
                    grabbingObject = foundMatData.grabbableObject;
                }
            }
            if (wasRightHandTouching)
            {
                if (RGrabbing)
                {
                    grabbingObject = foundMatData.grabbableObject;
                }
            }
        }
    }

    private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, out Rigidbody HitRigidbody)
    {
        slipPercentage = defaultSlideFactor;
        if (CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out tempIterativeHit))
        {
            firstPosition = endPosition;
            iterativeHitInfo = tempIterativeHit;
            float slidePercentage = GetSlidePercentage(iterativeHitInfo, out HitRigidbody);
            slipPercentage = (slidePercentage != defaultSlideFactor) ? slidePercentage : ((!singleHand) ? defaultSlideFactor : 0.001f);
            if (Physics.Raycast(startPosition, iterativeHitInfo.point - startPosition, out tempHitInfo, (iterativeHitInfo.point - startPosition).magnitude * 1.05f, locomotionEnabledLayers.value))
            {
                iterativeHitInfo = tempHitInfo;
            }
            movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - firstPosition, iterativeHitInfo.normal) * slipPercentage;
            if (CollisionsSphereCast(endPosition, sphereRadius, movementToProjectedAboveCollisionPlane, out endPosition, out tempIterativeHit))
            {
                iterativeHitInfo = tempIterativeHit;
                return true;
            }
            if (CollisionsSphereCast(movementToProjectedAboveCollisionPlane + firstPosition, sphereRadius, startPosition + movementVector - (movementToProjectedAboveCollisionPlane + firstPosition), out endPosition, out tempIterativeHit))
            {
                iterativeHitInfo = tempIterativeHit;
                return true;
            }
            endPosition = startPosition + movementVector;
            HitRigidbody = null;
            return true;
        }
        iterativeHitInfo = tempIterativeHit;
        endPosition = Vector3.zero;
        HitRigidbody = null;
        return false;
    }

    private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
    {
        MaxSphereSizeForNoOverlap(sphereRadius, startPosition, out maxSphereSize1);
        if (Physics.SphereCast(startPosition, maxSphereSize1, movementVector, out collisionsHitInfo, movementVector.magnitude, locomotionEnabledLayers.value))
        {
            finalPosition = startPosition + movementVector.normalized * collisionsHitInfo.distance;
            finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
            MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, out maxSphereSize2);
            if (Physics.SphereCast(startPosition, Mathf.Min(maxSphereSize1, maxSphereSize2), finalPosition - startPosition, out tempHitInfo, (finalPosition - startPosition).magnitude, locomotionEnabledLayers.value))
            {
                finalPosition = startPosition + tempHitInfo.distance * (finalPosition - startPosition).normalized;
            }
            if (Physics.Raycast(startPosition, finalPosition - startPosition, out tempHitInfo, (finalPosition - startPosition).magnitude, locomotionEnabledLayers.value))
            {
                finalPosition = startPosition;
            }
            return true;
        }
        if (Physics.Raycast(startPosition, movementVector, out collisionsHitInfo, movementVector.magnitude, locomotionEnabledLayers.value))
        {
            Debug.Log("movement check failed! we had to use the backup raycast. investigate the conditions that caused this to happen");
            finalPosition = startPosition;
            return true;
        }
        finalPosition = startPosition + movementVector;
        return false;
    }

    private Vector3 GetMoveSpacePositionDelta()
    {
        if(MSDetect.GetHighestPriority() != null)
        {
            MoveSpace ms = MSDetect.GetHighestPriority();
            return ms.currPosition - ms.lastPosition;
        }
        return new(0,0,0);
    }

    private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, out float overlapRadiusTest)
    {
        overlapRadiusTest = testRadius;
        overlapAttempts = 0;
        while (overlapAttempts < 1000)
        {
            if (Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, overlapColliders, locomotionEnabledLayers.value) > 0)
            {
                overlapRadiusTest *= 0.999f;
                overlapAttempts++;
                continue;
            }
            overlapRadiusTest *= 0.9995f;
            return true;
        }
        return false;
    }

    public float GetSlidePercentage(RaycastHit raycastHit, out Rigidbody HitRigidbody)
    {
        currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
        if (raycastHit.collider.gameObject.TryGetComponent(out Rigidbody rb))
        {
            HitRigidbody = rb;
        }
        else
        {
            HitRigidbody = null;
        }
        if (currentOverride != null)
        {
            currentMaterialIndex = currentOverride.overrideIndex;
            if (!@b.resources.materialData.materialData[currentMaterialIndex].overrideSlidePercent)
            {
                return defaultSlideFactor;
            }
            return @b.resources.materialData.materialData[currentMaterialIndex].slidePercent;
        }
        meshCollider = raycastHit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return defaultSlideFactor;
        }
        collidedMesh = meshCollider.sharedMesh;
        if (!meshTrianglesDict.TryGetValue(collidedMesh, out sharedMeshTris))
        {
            sharedMeshTris = collidedMesh.triangles;
            meshTrianglesDict.Add(collidedMesh, (int[])sharedMeshTris.Clone());
        }
        vertex1 = sharedMeshTris[raycastHit.triangleIndex * 3];
        vertex2 = sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
        vertex3 = sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
        if (raycastHit.collider.GetComponent<Renderer>().materials.Length > 1)
        {
            for (int i = 0; i < raycastHit.collider.GetComponent<Renderer>().materials.Length; i++)
            {
                collidedMesh.GetTriangles(trianglesList, i);
                for (int j = 0; j < trianglesList.Count; j += 3)
                {
                    if (trianglesList[j] == vertex1 && trianglesList[j + 1] == vertex2 && trianglesList[j + 2] == vertex3)
                    {
                        raycastHit.collider.GetComponent<Renderer>().GetMaterials(materialsList);
                        findMatName = materialsList[i].name;
                        findMatName = findMatName[..findMatName.IndexOf(' ')];
                        foundMatData = @b.resources.materialData.materialData.Find(matData => matData.matName == findMatName);
                        currentMaterialIndex = @b.resources.materialData.materialData.FindIndex(matData => matData.matName == findMatName);
                        if (currentMaterialIndex == -1)
                        {
                            currentMaterialIndex = 0;
                        }
                        if (!foundMatData.overrideSlidePercent)
                        {
                            return defaultSlideFactor;
                        }
                        return foundMatData.slidePercent;
                    }
                }
            }
            currentMaterialIndex = 0;
            return defaultSlideFactor;
        }
        raycastHit.collider.GetComponent<Renderer>().GetMaterials(materialsList);
        findMatName = materialsList[0].name;
        findMatName = findMatName[..findMatName.IndexOf(' ')];
        foundMatData = @b.resources.materialData.materialData.Find(matData => matData.matName == findMatName);
        currentMaterialIndex = @b.resources.materialData.materialData.FindIndex(matData => matData.matName == findMatName);
        if (currentMaterialIndex == -1)
        {
            currentMaterialIndex = 0;
        }
        if (!foundMatData.overrideSlidePercent)
        {
            return defaultSlideFactor;
        }
        return foundMatData.slidePercent;
    }

    private void StoreVelocities()
    {
        velocityIndex = (velocityIndex + 1) % velocityHistorySize;
        if (didATurn)
        {
            currentVelocity = Vector3.zero;
        }
        else
        {
            currentVelocity = (base.transform.position - lastPosition) / calcDeltaTime;
        }
        velocityHistory[velocityIndex] = currentVelocity;
        denormalizedVelocityAverage = Vector3.zero;
        for (int i = 0; i < velocityHistory.Length; i++)
        {
            denormalizedVelocityAverage += velocityHistory[i];
        }
        denormalizedVelocityAverage /= (float)velocityHistorySize;
        lastPosition = base.transform.position;
        didATurn = false;
    }
}
