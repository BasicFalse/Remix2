using UnityEngine;

public class RangeFling : MonoBehaviour
{
    public GameObject targetCube;        // The target position for the fling
    public float flingSpeed = 5f;        // Adjustable fling speed
    public float flingCooldown = 2f;     // Cooldown time to prevent multiple flings
    public AnimationCurve flingArc;      // The arc of the fling
    public RangeShape rangeShape = RangeShape.Sphere; // Current shape of the range
    public Vector3 boxSize = new Vector3(5f, 5f, 5f); // Dimensions of the cube range
    public float sphereRadius = 5f;      // Radius for the spherical range

    private bool isOnCooldown = false;

    public enum RangeShape
    {
        Sphere,
        Cube
    }

    private void FixedUpdate()
    {
        if (isOnCooldown) return; // Skip if still on cooldown

        // Check detection based on the selected shape
        bool isPlayerInRange = false;
        Collider[] colliders = null;

        if (rangeShape == RangeShape.Sphere)
        {
            colliders = Physics.OverlapSphere(transform.position, sphereRadius);
        }
        else if (rangeShape == RangeShape.Cube)
        {
            colliders = Physics.OverlapBox(transform.position, boxSize / 2f);
        }

        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.attachedRigidbody;

                if (rb != null) // Ensure we only affect objects with rigidbodies
                {
                    StartCoroutine(FlingToTarget(rb));
                    isPlayerInRange = true;
                    break; // Stop after finding one valid player
                }
            }
        }

        if (isPlayerInRange) return; // Don't perform further checks
    }

    private System.Collections.IEnumerator FlingToTarget(Rigidbody rb)
    {
        isOnCooldown = true;

        // Get starting and target positions
        Vector3 startPosition = rb.position;
        Vector3 targetPosition = targetCube.transform.position;

        float elapsedTime = 0f;
        float totalFlightTime = Vector3.Distance(startPosition, targetPosition) / flingSpeed;

        while (elapsedTime < totalFlightTime)
        {
            float progress = elapsedTime / totalFlightTime;

            // Calculate the current position along the arc
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y += flingArc.Evaluate(progress); // Adjust height using the animation curve

            rb.MovePosition(currentPosition); // Smoothly move the player

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Ensure the player lands exactly at the target position
        rb.MovePosition(targetPosition);

        // Reset cooldown after a short delay
        yield return new WaitForSeconds(flingCooldown);
        isOnCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the selected shape range
        Gizmos.color = Color.cyan;

        if (rangeShape == RangeShape.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
        else if (rangeShape == RangeShape.Cube)
        {
            Gizmos.DrawWireCube(transform.position, boxSize);
        }

        // Visualize the fling arc
        if (targetCube != null)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = targetCube.transform.position;
            int arcPoints = 20; // Number of points to define the arc

            Gizmos.color = Color.magenta;
            for (int i = 0; i < arcPoints; i++)
            {
                float t1 = i / (float)arcPoints;
                float t2 = (i + 1) / (float)arcPoints;

                Vector3 p1 = Vector3.Lerp(startPosition, targetPosition, t1);
                Vector3 p2 = Vector3.Lerp(startPosition, targetPosition, t2);

                p1.y += flingArc.Evaluate(t1);
                p2.y += flingArc.Evaluate(t2);

                Gizmos.DrawLine(p1, p2); // Draw lines connecting arc points
            }
        }
    }
}
