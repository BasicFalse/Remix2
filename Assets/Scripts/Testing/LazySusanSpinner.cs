using System.Collections.Generic;
using easyInputs;
using UnityEngine;

public class LazySusanSpinner : MonoBehaviour
{
    [Header("Spinner Setup")]
    public Transform spinner; // the rotating platform
    public List<Collider> grabColliders; // rim colliders

    [Header("Settings")]
    public float smoothness = 10f;      // responsiveness
    public float releaseDamping = 0.5f; // momentum slowdown
    public float gripThreshold = 0.5f;  // grip activation threshold

    private Transform grabbingHand;
    private EasyHand currentHand; // which hand is grabbing
    private bool isGrabbed = false;
    private float lastHandAngle;
    private float spinVelocity;

    private Dictionary<Collider, Transform> touchingHands = new Dictionary<Collider, Transform>();

    void Update()
    {
        if (isGrabbed && grabbingHand != null)
        {
            Vector3 center = spinner.position;
            Vector3 handDir = grabbingHand.position - center;
            handDir.y = 0;

            float handAngle = Mathf.Atan2(handDir.x, handDir.z) * Mathf.Rad2Deg;

            if (lastHandAngle != 0)
            {
                float delta = Mathf.DeltaAngle(lastHandAngle, handAngle);
                spinVelocity = Mathf.Lerp(spinVelocity, delta * smoothness, Time.deltaTime * smoothness);
                spinner.Rotate(Vector3.up, spinVelocity * Time.deltaTime, Space.World);
            }

            lastHandAngle = handAngle;

            // release if grip is no longer held
            if (!IsGripping(currentHand))
                Release();
        }
        else
        {
            // momentum carry
            spinner.Rotate(Vector3.up, spinVelocity * Time.deltaTime, Space.World);
            spinVelocity = Mathf.Lerp(spinVelocity, 0, Time.deltaTime * releaseDamping);
        }

        // check for new grab
        if (!isGrabbed)
        {
            foreach (var kv in touchingHands)
            {
                Transform hand = kv.Value;
                EasyHand handType = hand.CompareTag("LeftHand") ? EasyHand.LeftHand : EasyHand.RightHand;

                if (IsGripping(handType))
                {
                    Grab(hand, handType);
                    break;
                }
            }
        }
    }

    void Grab(Transform hand, EasyHand handType)
    {
        grabbingHand = hand;
        currentHand = handType;
        isGrabbed = true;
        lastHandAngle = 0;
        spinVelocity = 0;
    }

    void Release()
    {
        grabbingHand = null;
        isGrabbed = false;
        lastHandAngle = 0;
    }

    private bool IsGripping(EasyHand hand)
    {
        return EasyInputs.GetGripButtonFloat(hand) > gripThreshold;
    }

    void OnTriggerEnter(Collider other)
    {
        if (grabColliders.Contains(other))
        {
            Transform hand = other.transform.root;
            if (hand.CompareTag("LeftHand") || hand.CompareTag("RightHand"))
            {
                if (!touchingHands.ContainsKey(other))
                    touchingHands.Add(other, hand);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (touchingHands.ContainsKey(other))
            touchingHands.Remove(other);
    }
}
