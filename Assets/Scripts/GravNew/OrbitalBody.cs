using Unity.Mathematics;
using UnityEngine;

namespace GravNew{
[RequireComponent(typeof(Rigidbody))]
public class OrbitalBody : MonoBehaviour
{
    public Transform star;              // the central star
    public float planetMass = 1f;       // mass of this planet
    public float starMass = 1000f;      // mass of the star
    public float gravitationalConstant = 0.1f;
    public Vector3 Spin;
    public bool2 ApplyOrbitAndOrSpin;
    public bool autoInitVelocity = true;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        if (autoInitVelocity && star != null)
        {
            // Vector from star to planet
            Vector3 offset = rb.position - star.position;
            float r = offset.magnitude;

            // orbital speed for circular orbit
            float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * starMass / r);

            // direction perpendicular to radius vector (choose "up" axis as reference)
            Vector3 orbitalDir = Vector3.Cross(offset.normalized, Vector3.up).normalized;

            if(ApplyOrbitAndOrSpin.x)
            rb.linearVelocity = orbitalDir * orbitalSpeed;
            if(ApplyOrbitAndOrSpin.y)
            rb.angularVelocity = Spin;
        }
    }

    void FixedUpdate()
    {
        if (star == null) return;

        Vector3 dir = (star.position - rb.position).normalized;
        float distSqr = (star.position - rb.position).sqrMagnitude;

        // gravitational force toward star
        Vector3 force = dir * gravitationalConstant * (starMass) / distSqr;

        if(ApplyOrbitAndOrSpin.x)
        rb.AddForce(force, ForceMode.Acceleration);
    }


    public Vector3 GetVelocity()
    {

        return rb.linearVelocity + rb.angularVelocity;
    }
    
    public Vector3 GetVelocityAtPoint(Vector3 point)
    {
        Vector3 linear = rb.linearVelocity;
        Vector3 angular = Vector3.Cross(rb.angularVelocity, point - rb.worldCenterOfMass);
        return linear + angular;
    }
}
}