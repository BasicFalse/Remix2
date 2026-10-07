using UnityEngine;

namespace GorillaLocomotion.Swimming{
public class VRSwimming : MonoBehaviour
{
    [Header("References")]
    public Rigidbody playerRigidbody;
    public GorillaVibration GV;
    public Transform leftHand;
    public Transform rightHand;
    public GameObject waterOverlay;

    public WaterDetector headWater;
    public WaterDetector bodyWater;
    public WaterDetector leftHandWater;
    public WaterDetector rightHandWater;

    [Header("Swimming Tuning")]
    public float swimForce = 8f;
    public float maxSpeed = 6f;
    public float waterDrag = 4f;
    public float minStrokeSpeed = 0.02f;

    [Header("Hand Normals")]
    [Tooltip("Which axis of the left hand represents the palm normal")]
    public Vector3 leftHandNormalAxis = Vector3.up;

    [Tooltip("Which axis of the right hand represents the palm normal")]
    public Vector3 rightHandNormalAxis = Vector3.up;

    private Vector3 lastLeftPos;
    private Vector3 lastRightPos;

    void Start()
    {
        lastLeftPos = leftHand.position;
        lastRightPos = rightHand.position;
    }

    void Update()
    {
        if(waterOverlay) waterOverlay.SetActive(headWater.InWater);
    }

    void FixedUpdate()
    {
        Vector3 totalForce = Vector3.zero;
        bool anyHandInWater = false;

        if (leftHandWater.InWater)
        {
            Vector3 lhf = CalculateHandForce(leftHand, ref lastLeftPos, leftHandNormalAxis);
            totalForce += lhf;
            GV.StartVibration(true, lhf.magnitude, 0.1f);
            anyHandInWater = true;
        }
        else
        {
            lastLeftPos = leftHand.position;
        }

        if (rightHandWater.InWater)
        {
            Vector3 rhf = CalculateHandForce(rightHand, ref lastRightPos, rightHandNormalAxis);
            totalForce += rhf;
            GV.StartVibration(false, rhf.magnitude, 0.1f);
            anyHandInWater = true;
        }
        else
        {
            lastRightPos = rightHand.position;
        }

        if (anyHandInWater)
        {
            if (totalForce != Vector3.zero)
            {
                playerRigidbody.AddForce(
                    totalForce * swimForce,
                    ForceMode.Acceleration
                );
            }

            // Clamp speed like Gorilla Tag
            if (playerRigidbody.linearVelocity.magnitude > maxSpeed)
            {
                playerRigidbody.linearVelocity =
                    playerRigidbody.linearVelocity.normalized * maxSpeed;
            }
        }

        playerRigidbody.linearDamping = anyHandInWater ? waterDrag : 0f;
    }

    Vector3 CalculateHandForce(Transform hand, ref Vector3 lastPos, Vector3 handNormalAxis)
    {
        Vector3 delta = hand.position - lastPos;
        float deltaFlat = Vector3.Distance(hand.position, lastPos);
        Vector3 handVelocity = delta / Time.fixedDeltaTime;
        lastPos = hand.position;

        float speed = handVelocity.magnitude;
        if (speed < minStrokeSpeed)
            return Vector3.zero;

        // Get palm normal in world space
        Vector3 palmNormal = hand.TransformDirection(handNormalAxis).normalized;

        // How well the hand is pushing against water
        float alignment = Vector3.Dot(-handVelocity.normalized, palmNormal);

        if (alignment <= 0f)
            return Vector3.zero;

        // Final force direction & strength
        Vector3 force = alignment * speed * palmNormal;

        return force;
    }
}
}