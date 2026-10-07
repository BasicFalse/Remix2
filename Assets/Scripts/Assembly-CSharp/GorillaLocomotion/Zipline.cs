using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using SplineTools;

[RequireComponent(typeof(SplineContainer))]
public class Zipline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public AudioClip ZipClip;
    public float gravity = -9.81f;
    public float drag = 2f;
    public float velocity;
    public float t;

    public Vector3 WorldPosition { get; private set; }

    void Reset()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    void Update()
    {
        if (splineContainer == null) return;

        float deltaTime = Time.deltaTime;
        var spline = splineContainer.Spline;

        // Get tangent direction at current t
        Vector3 tangent = spline.EvaluateTangent(t);
        tangent.Normalize();

        // Project gravity onto spline direction
        float gravityAlongSpline = Vector3.Dot(Vector3.up * gravity, tangent);

        // Apply gravity
        velocity += gravityAlongSpline * deltaTime;

        // Apply drag
        velocity *= 1f / (1f + drag * deltaTime);

        // Move along spline
        float splineLength = spline.GetLength();
        t += (velocity / splineLength) * deltaTime;

        // Clamp to spline
        t = Mathf.Clamp01(t);

        // Stop velocity at ends
        if (t == 0f || t == 1f)
            velocity = 0f;

        // Update world position
        WorldPosition = splineContainer.transform.TransformPoint(
            spline.EvaluatePosition(t)
        );

        // Move this object to simulated point
        //transform.position = WorldPosition;
    }

    
    /// <summary>
    /// Sets the point t where it is closest to the given world pos.
    /// </summary>
    /// <param name="worldPos"></param>
    public void SetToClosestPoint(Vector3 worldPos)
    {
        if (splineContainer == null)
            return;

        var spline = splineContainer.Spline;

        Vector3 localPoint = splineContainer.transform.InverseTransformPoint(worldPos);

        Ray ray = new(localPoint + Vector3.up * 100f, Vector3.down);


        SplineUtility.GetNearestPoint(
            spline,
            ray,
            out _,
            out float nearestT
        );

        t = nearestT;
        velocity = 0f;

        WorldPosition = splineContainer.transform.TransformPoint(
            spline.EvaluatePosition(t)
        );
    }

    /// <summary>
    /// Sets the Velocity of t based on a given Rigidbody
    /// </summary>
    /// <param name="rb"></param>
    public void SetVelocityFromRigidbody(Rigidbody rb)
    {
        if (splineContainer == null || rb == null)
            return;

        var spline = splineContainer.Spline;

        // Get tangent at current t (local space)
        Vector3 localTangent = spline.EvaluateTangent(t);
        localTangent.Normalize();

        // Convert tangent to world space
        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);
        worldTangent.Normalize();

        // Project rigidbody velocity onto spline direction
        velocity = Vector3.Dot(rb.linearVelocity, worldTangent);
    }

    /// <summary>
    /// Sets the position of t along the spline based on a float.
    /// </summary>
    /// <param name="newT"></param>
    public void SetT(float newT)
    {
        t = Mathf.Clamp01(newT);
        velocity = 0f;
    }

    /// <summary>
    /// Gets the world velocity of the point t of the zipline.
    /// </summary>
    /// <returns>World Velocity</returns>
    public Vector3 GetWorldVelocity()
    {
        if (splineContainer == null)
            return Vector3.zero;

        var spline = splineContainer.Spline;

        // Get local tangent
        Vector3 localTangent = spline.EvaluateTangent(t);
        localTangent.Normalize();

        // Convert to world direction
        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);
        worldTangent.Normalize();

        // Multiply by spline velocity
        return worldTangent * velocity;
    }

}