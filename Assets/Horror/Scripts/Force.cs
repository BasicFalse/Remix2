using UnityEngine;

public class Force : MonoBehaviour
{
    // Direction and magnitude of the force to be applied
    public Vector3 forceDirection = Vector3.up; // Default direction is up
    public float forceAmount = 10f;             // Default force amount

    // The Rigidbody that will receive the force
    public Rigidbody targetRigidbody;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has the "Player" tag
        if (other.CompareTag("GorillaPlayer"))
        {
            ApplyForce();
        }
    }

    private void ApplyForce()
    {
        // Ensure the targetRigidbody is assigned
        if (targetRigidbody != null)
        {
            // Normalize the force direction and multiply by the force amount
            Vector3 force = forceDirection.normalized * forceAmount;

            // Move the kinematic Rigidbody directly by changing its position
            if (targetRigidbody.isKinematic)
            {
                targetRigidbody.MovePosition(targetRigidbody.position + force * Time.deltaTime);
            }
            else
            {
                // Apply the force for non-kinematic rigidbodies
                targetRigidbody.AddForce(force, ForceMode.Impulse);
            }

            // Log a message to confirm the force application
            Debug.Log($"Applied force: {force} to {targetRigidbody.name}");
        }
        else
        {
            Debug.LogWarning("Target Rigidbody is not assigned.");
        }
    }
}