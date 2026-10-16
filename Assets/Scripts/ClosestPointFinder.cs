using UnityEngine;

public class ClosestPointFinder : MonoBehaviour
{
    public float searchRadius = 20f;
    public LayerMask geometryLayer;

    public Vector3 GetClosestPointOnGeometry(Vector3 targetPosition)
    {
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, searchRadius, geometryLayer);
        
        Vector3 closestPoint = Vector3.zero;
        float closestDistanceSqr = float.MaxValue;
        bool foundGeometry = false;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Vector3 pointOnCollider = hitColliders[i].ClosestPoint(targetPosition); 
            float distanceSqr = (pointOnCollider - targetPosition).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestPoint = pointOnCollider;
                foundGeometry = true;
            }
        }

        return foundGeometry ? closestPoint : targetPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        Gizmos.DrawLine(transform.position, GetClosestPointOnGeometry(transform.position));
    }
}
