using System.Collections.Generic;
using UnityEngine;

public class FloatingWhiteBall : MonsterBase
{
    [System.Serializable]
    public class LegSegment
    {
        public Transform LegEnd;
        public LineRenderer LR;

        public void ApplyLegChanges(Vector3 bass)
        {
            Vector3[] poses = {LegEnd.position, bass};
            LR.SetPositions(poses);
        }
    }

    public LegSegment[] LegEnds;
    public float maxGrabRange;
    public LayerMask ValidSurfaceLayers;
    
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public int waitTime = 30;
    public int waiter = 31;
    public Vector3 lastPlannedVec;
    public Transform[] Points;

    public Vector3 targetPlayer;
    public bool isCollided = false;

    void FixedUpdate()
    {
        foreach(LegSegment ls in LegEnds)
        {
            float dist = Vector3.Distance(ls.LegEnd.position, transform.position);
            Vector3 newPos = ls.LegEnd.position;
            if(dist > maxGrabRange || dist < 3)
            {
                bool didray = false;
                int tries = 0;
                while (!didray && tries < 1000)
                {
                    if (RaycastRandomDirection(transform, maxGrabRange, out RaycastHit hit, ValidSurfaceLayers))
                    {
                        //Debug.Log("Hit " + hit.collider.name);
                        newPos = hit.point;
                    }
                    if(Vector3.Distance(transform.position, newPos) < maxGrabRange)
                    {
                        didray = true;
                    }
                    tries++;
                }

                if(tries >= 999)
                {
                    newPos = transform.position;
                }
                
                //else
                //{
                //    newPos = transform.position;
                //}
            }
            ls.LegEnd.position = newPos;
            ls.ApplyLegChanges(transform.position);
        }
        
    }

    public override void OfflineMovement()
    {
        targetPlayer = GetPlayerPosition();
        
        List<Vector3> newpoints = new();
        foreach(Transform t in Points)
        {
            newpoints.Add(t.position);
        }

        if(Vector3.Distance(targetPlayer,transform.position) < chaseRange)
        {
            if (CanSeePoint(transform.position, targetPlayer))
            {
                Move(targetPlayer);
            }
            else
            {
                Move(SmartPathing(targetPlayer, transform.position, newpoints.ToArray()));
            }
        }
        else
        {
            if(CanSeePoint(transform.position, initialPosition))
            {
                Move(initialPosition);
            }
            else
            {
                Move(SmartPathing(initialPosition, transform.position, newpoints.ToArray()));
            }
        } 
    }

    public override void OnlineMasterMovement()
    {
        targetPlayer = ClosestPlayer();

        List<Vector3> newpoints = new();
        foreach(Transform t in Points)
        {
            newpoints.Add(t.position);
        }

        if(Vector3.Distance(targetPlayer,transform.position) < chaseRange)
        {
            if (CanSeePoint(transform.position, targetPlayer))
            {
                Move(targetPlayer);
            }
            else
            {
                Move(SmartPathing(targetPlayer, transform.position, newpoints.ToArray()));
            }
        }
        else
        {
            if(CanSeePoint(transform.position, initialPosition))
            {
                Move(initialPosition);
            }
            else
            {
                Move(SmartPathing(initialPosition, transform.position, newpoints.ToArray()));
            }
        } 
    }

    void Move(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
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
    public static bool RaycastRandomDirection(Transform origin, float maxDistance, out RaycastHit hit, LayerMask mask)
    {
        // Random direction in 3D space
        Vector3 randomDir = Random.onUnitSphere;

        return Physics.Raycast(origin.position, randomDir, out hit, maxDistance, mask);
    }

}
