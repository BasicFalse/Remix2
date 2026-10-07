using UnityEngine;
using System.Collections.Generic;

public class SingleFileFollowerChain : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject followerPrefab;
    public int followerCount = 5;

    [Header("Follow Source")]
    public bool followExternalTarget = false;
    public Transform externalTarget;

    [Header("Movement")]
    public float separationDistance = 1.5f;
    public float followStrength = 10f;
    public float distanceMultiplier = 0.15f;
    public float maxCorrectionFactor = 1.2f;

    [Header("History")]
    public int maxHistoryPoints = 600;

    private readonly List<Transform> followers = new();
    private readonly List<Vector3> positionHistory = new();

    private Transform leader;
    private Vector3 lastRecordedPos;

    void Start()
    {
        ResolveLeader();
        SpawnFollowers();

        lastRecordedPos = leader.position;
        positionHistory.Add(lastRecordedPos);
    }

    void FixedUpdate()
    {
        if (leader == null) return;

        RecordHistorySubdivided();
        MoveFollowersSimultaneous();
    }

    void ResolveLeader()
    {
        leader = followExternalTarget ? externalTarget : transform;

        if (leader == null)
            Debug.LogError($"{name}: Leader is null.");
    }

    void RecordHistorySubdivided()
    {
        Vector3 current = leader.position;
        float distance = Vector3.Distance(lastRecordedPos, current);

        if (distance < 0.0001f) return;

        float stepSize = separationDistance * 0.5f;
        int steps = Mathf.CeilToInt(distance / stepSize);

        for (int i = 1; i <= steps; i++)
        {
            Vector3 point = Vector3.Lerp(lastRecordedPos, current, i / (float)steps);
            positionHistory.Insert(0, point);

            if (positionHistory.Count > maxHistoryPoints)
                positionHistory.RemoveAt(positionHistory.Count - 1);
        }

        lastRecordedPos = current;
    }

    void MoveFollowersSimultaneous()
    {
        Vector3[] nextPositions = new Vector3[followers.Count];
        Quaternion[] nextRotations = new Quaternion[followers.Count];

        // Step 1: Compute new positions & rotations
        for (int i = 0; i < followers.Count; i++)
        {
            float desiredDistance = separationDistance * (i + 1);
            Vector3 targetPos = GetHistoryPointAtDistance(desiredDistance);

            Vector3 error = targetPos - followers[i].position;
            if (!IsFinite(error))
            {
                nextPositions[i] = followers[i].position;
                nextRotations[i] = followers[i].rotation;
                continue;
            }

            float scale = 1f + Mathf.Log(1f + desiredDistance) * distanceMultiplier;
            Vector3 correction = error * followStrength * scale * Time.fixedDeltaTime;
            float maxCorrection = error.magnitude * maxCorrectionFactor;
            correction = Vector3.ClampMagnitude(correction, maxCorrection);

            nextPositions[i] = followers[i].position + correction;

            if (error.sqrMagnitude > 0.0001f)
                nextRotations[i] = Quaternion.Slerp(followers[i].rotation,
                    Quaternion.LookRotation(error),
                    followStrength * scale * Time.fixedDeltaTime);
            else
                nextRotations[i] = followers[i].rotation;
        }

        // Step 2: Apply all positions & rotations at once
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].position = nextPositions[i];
            followers[i].rotation = nextRotations[i];
        }
    }

    Vector3 GetHistoryPointAtDistance(float distance)
    {
        float accumulated = 0f;

        for (int i = 0; i < positionHistory.Count - 1; i++)
        {
            float segment = Vector3.Distance(positionHistory[i], positionHistory[i + 1]);
            accumulated += segment;

            if (accumulated >= distance)
                return positionHistory[i + 1];
        }

        return positionHistory[positionHistory.Count - 1];
    }

    bool IsFinite(Vector3 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }

    void SpawnFollowers()
    {
        followers.Clear();

        for (int i = 0; i < followerCount; i++)
        {
            GameObject obj = Instantiate(followerPrefab, leader.position, leader.rotation);
            followers.Add(obj.transform);
        }
    }
}
