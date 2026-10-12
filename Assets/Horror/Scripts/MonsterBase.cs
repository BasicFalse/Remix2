using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using GorillaNetworking;

public class MonsterBase : MonoBehaviourPunCallbacks
{

    public bool addedToMisc = false;
    public bool init = false;
    public bool resetPosition = true;
    public Vector3 initialPosition;
    public Light MonLight;
    
    protected virtual void Start()
    {
        initialPosition = transform.position;
        EnemyManager.inst.AddMonster(this);
        init = true;
        MiscGrabs.grab.monsters.Add(this);
        if (MiscGrabs.grab.monsters.Contains(this))
        {
            addedToMisc = true;
        }
    }
    

    /// <summary>
    /// Updates the monster in question.
    /// behavior varies on a few factors.
    /// </summary>
    public virtual void UpdateMonster()
    {
        RegardlessMovement();
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnlineMasterMovement();
            }
            else
            {
                OnlineSlaveMovement();
            }
        }
        else
        {
            OfflineMovement();
        }
    }

    /// <summary>
    ///  What is the monster doing when the client is offline?
    /// </summary>
    public virtual void OfflineMovement(){}

    /// <summary>
    /// What is the monster doing when the client is online and in control of the room?
    /// </summary>
    public virtual void OnlineMasterMovement(){}

    /// <summary>
    /// What is the monster doing when the client in online and not in control of the room?
    /// This is usually taken care of by the photon view.
    /// </summary>
    public virtual void OnlineSlaveMovement(){}

    /// <summary>
    /// This is what the monster is doing weather or not the player is in a room.
    /// </summary>
    public virtual void RegardlessMovement(){}


    /// <summary>
    /// Premade pathing algorithm, can be costly to run every frame.
    /// Looks for Target from the CurrentPos and returns a vector that is the next path of action in PathPoints or returns the Target vector if in view.
    /// </summary>
    /// <param name="Target"> Where to go </param>
    /// <param name="CurrentPos"> Where are we </param>
    /// <param name="PathPoints"> Available Pathing Points </param>
    /// <returns></returns>
    public virtual Vector3 SmartPathing(Vector3 Target, Vector3 CurrentPos, Vector3[] PathPoints = null)
    {
        if (CanSeePoint(CurrentPos, Target) || PathPoints == null)
        {
            return Target;
        }


        Vector3 nextBest = PathPoints[1];
        List<Vector3> nearbyPoints = new();

        foreach (Vector3 v in PathPoints)
        {
            if(Vector3.Distance(CurrentPos,v) < Vector3.Distance(CurrentPos, nextBest))
            {
                nextBest = v;
            }
            if(CanSeePoint(v,Target))
            {
                nearbyPoints.Add(v);
            }
        }
        
        foreach(Vector3 v in nearbyPoints)
        {
            if(CanSeePoint(CurrentPos, v))
            {
                return v;
            }
        }

        List<Vector3> nearby2Points = new();

        foreach (Vector3 v in PathPoints)
        {
            if(CanSeeAnyPoint(v,nearbyPoints.ToArray()))
            {
                nearby2Points.Add(v);
            }
        }

        foreach(Vector3 v in nearby2Points)
        {
            if(CanSeePoint(CurrentPos, v))
            {
                return v;
            }
        }

        List<Vector3> nearby3Points = new();

        foreach (Vector3 v in PathPoints)
        {
            if(CanSeeAnyPoint(v,nearby2Points.ToArray()))
            {
                nearby3Points.Add(v);
            }
        }

        foreach(Vector3 v in nearby3Points)
        {
            if(CanSeePoint(CurrentPos, v))
            {
                return v;
            }
        }

        List<Vector3> nearby4Points = new();

        foreach (Vector3 v in PathPoints)
        {
            if(CanSeeAnyPoint(v,nearby3Points.ToArray()))
            {
                nearby4Points.Add(v);
            }
        }

        foreach(Vector3 v in nearby4Points)
        {
            if(CanSeePoint(CurrentPos, v))
            {
                return v;
            }
        }


        foreach (Vector3 v in PathPoints)
        {
            //float DBNextPoint = Vector3.Distance(CurrentPos , v);
            //float DBMonAndHome = Vector3.Distance(CurrentPos , HomePos);
            float DBHomeAndNextPoint = Vector3.Distance(Target , v);

            if (CanSeePoint(CurrentPos, v))
            {
                if(DBHomeAndNextPoint < Vector3.Distance(nextBest, Target)){
                nextBest = v;}
                if (CanSeePoint(v, Target))
                {
                    return v;
                }

                foreach(Vector3 v2 in nearbyPoints)
                {
                    if (CanSeePoint(v, v2))
                    {
                        return v;
                    }
                }
            }
            
        }
        
        return nextBest;
    }

    /// <summary>
    /// Where is the closest player relative to us?
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 ClosestPlayer()
    {
        if (!GorillaComputer.instance.isInRoom)
            return MiscGrabs.grab.GetPlayerPosition();

        float closestDistance = Vector3.Distance(transform.position, MiscGrabs.grab.GetPlayerPosition());
        Transform closest = null;

        foreach (Transform p in MiscGrabs.grab.PlayerCameras)
        {
            if (p == null) break;

            float distance = Vector3.Distance(transform.position, p.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = p;
            }
        }

        return closest.position;
    }

    /// <summary>
    /// What is the distance to the closest player?
    /// </summary>
    /// <returns></returns>
    public virtual float DistanceToClosestPlayer()
    {
        return Vector3.Distance(ClosestPlayer(), transform.position);
    }
    public virtual float DistanceToClosestPlayer(Vector3 position)
    {
        return Vector3.Distance(ClosestPlayer(), position);
    }

    public virtual bool CanSeePoint(Vector3 PosA, Vector3 PosB)
    {
        Vector3 dir = (PosB - PosA).normalized;
        float distance = Vector3.Distance(PosA, PosB);
        Vector3 rayStart = PosA + dir * 0.1f; // small offset to prevent self-collision
        return !Physics.Raycast(rayStart, dir, distance);
    }

    public virtual bool CanSeeAnyPoint(Vector3 PosA, Vector3[] poss)
    {
        foreach(Vector3 v in poss)
        {
            if (CanSeePoint(PosA, v))
            {
                return true;
            }
        }
        return false;
    }

    public virtual Vector3 ClosestPoint(Vector3 curr, Vector3[] points)
    {
        Vector3 closest = points[1];

        foreach(Vector3 v in points)
        {
            if(Vector3.Distance(closest, curr) > Vector3.Distance(v, curr))
            {
                closest = v;
            }
        }

        return closest;
    }

    public virtual Vector3 GetPlayerPosition()
    {
        if(MiscGrabs.grab.DoesPlayerExist())
        {
            return MiscGrabs.grab.GetPlayerPosition();
        }
        else
        {
            return new Vector3();
        }
    }

    public virtual void ToggleLight(bool t)
    {
        if(MonLight)
        {
            MonLight.enabled = t;
        }
    }
}
