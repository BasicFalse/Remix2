using UnityEngine;

public class TitleMon : MonsterBase
{
    public float moveSpeed = 2f;

    public override void OfflineMovement()
    {
        MoveAndRotateToward(initialPosition);
    }

    void MoveAndRotateToward(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            float rotationSpeed = moveSpeed * 5f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    public override void OnEnable()
    {
        //base.OnEnable();
    }
    
}
