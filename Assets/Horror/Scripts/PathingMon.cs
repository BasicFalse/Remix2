using System.Collections.Generic;
using UnityEngine;

public class PathingMon : MonsterBase
{
    public Transform Home;
    public Transform[] Points;
    public Vector3 lastPlannedVec;
    public float chaseRange;
    public float moveSpeed;
    public float rotationSensitivity;
    public int waitTime = 30;
    public int waiter = 31;
    public bool coustomHome;

    public override void OfflineMovement()
    {
        List<Vector3> newpoints = new();
        foreach(Transform t in Points)
        {
            newpoints.Add(t.position);
        }

        Vector3 playerPos = ClosestPlayer();

        if(Vector3.Distance(playerPos,transform.position) < chaseRange)
        {
            if (CanSeePoint(transform.position, playerPos))
            {
                MoveAndRotateToward(playerPos);
            }
            else
            {
                MoveAndRotateToward(SmartPathing(playerPos, transform.position, newpoints.ToArray()));
            }
        }
        else
        {
            if (coustomHome)
            {
                if(CanSeePoint(transform.position, Home.position))
                {
                    MoveAndRotateToward(Home.position);
                }
                else
                {
                    MoveAndRotateToward(SmartPathing(Home.position, transform.position, newpoints.ToArray()));
                }
            }
            else
            {
                if(CanSeePoint(transform.position, initialPosition))
                {
                    MoveAndRotateToward(initialPosition);
                }
                else
                {
                    MoveAndRotateToward(SmartPathing(initialPosition, transform.position, newpoints.ToArray()));
                }
            }
        } 
    }

    public override Vector3 SmartPathing(Vector3 HomePos, Vector3 CurrentPos, Vector3[] PathPoints)
    {
        if(waiter > waitTime)
        {
            waiter = 0;
            lastPlannedVec = base.SmartPathing(HomePos, CurrentPos, PathPoints);
        }
        else
        {
            waiter++;
        }

        return lastPlannedVec;
    }

    void MoveAndRotateToward(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            float rotationSpeed = moveSpeed * rotationSensitivity;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }
}
