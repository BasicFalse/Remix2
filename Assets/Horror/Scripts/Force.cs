using UnityEngine;

public class Force : MonoBehaviour
{
    public Vector3 forceDirection = Vector3.up;
    public float forceAmount = 10f;

    public Rigidbody targetRigidbody => PlayerBase.i.resources.RB;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GorillaPlayer"))
        {
            ApplyForce();
        }
    }

    private void ApplyForce()
    {
        if (targetRigidbody != null)
        {
            Vector3 force = forceDirection.normalized * forceAmount;

            if (targetRigidbody.isKinematic)
            {
                targetRigidbody.MovePosition(targetRigidbody.position + force * Time.deltaTime);
            }
            else
            {
                targetRigidbody.AddForce(force, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogWarning("Player RB not found.");
        }
    }
}