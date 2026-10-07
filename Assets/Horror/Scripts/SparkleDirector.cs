using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SparkleSwarm))]
public class SparkleDirector : MonsterBase
{
    public enum SparkleMood { Looping, Hunting, Killing}
    public SplineContainer LoopingPath;
    public Transform SparkleTarget;
    public SparkleSwarm Swarm;
    public SparkleMood CurrentMood;
    public float sightDistance;
    public float chaseDistance;
    public float speed = 1f;
    private float t;
    private float splineLength;
    public SparkleBehaviorContainer CalmMood;
    public SparkleBehaviorContainer HuntingMood;
    public SparkleBehaviorContainer KillingMood;

    new void Start()
    {
        if (LoopingPath == null) return;
        splineLength = LoopingPath.Spline.GetLength();
        Swarm.SetMood(CalmMood);
        base.Start();
    }

    public void FixedUpdate()
    {
        transform.position=Swarm.averagePosition;
        switch (CurrentMood)
        {
            case SparkleMood.Looping:
            LoopingLogic();
            break;

            case SparkleMood.Hunting:
            HuntingLogic();
            break;

            case SparkleMood.Killing:
            break;
        }
        
    }

    public void LoopingLogic()
    {
        if (DistanceToClosestPlayer() < chaseDistance)
        {
            Swarm.SetMood(HuntingMood);
            CurrentMood = SparkleMood.Hunting;
        }

        if (LoopingPath == null || splineLength <= 0f) return;

        t += (speed / splineLength) * Time.deltaTime;
        t %= 1f;

        float3 localPos = LoopingPath.Spline.EvaluatePosition(t);
        Vector3 worldPos = LoopingPath.transform.TransformPoint(localPos);

        SparkleTarget.position = worldPos;
    }

    public void HuntingLogic()
    {
        if(DistanceToClosestPlayer() > sightDistance)
        {
            Swarm.SetMood(CalmMood);
            CurrentMood = SparkleMood.Looping;
        }
    }
}   
