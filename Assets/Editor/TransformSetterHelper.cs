using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PosRotPair
{
    public Vector3 Position;
    public Quaternion Rotation;
}

[ExecuteAlways]
public class TransformSetterHelper : MonoBehaviour
{
    public bool recordPoses;
    public bool applyPoses;
    public List<Transform> Objects;
    public List<PosRotPair> RecordedPoses;
    void Update()
    {
        if (recordPoses)
        {
            recordPoses = false;
            if(Objects.Count == 0){ Debug.Log("Object selection is empty."); return; }

            for(int i = 0; i < Objects.Count; i++)
            {
                PosRotPair newPair = new()
                {
                    Position = Objects[i].position,
                    Rotation = Objects[i].rotation
                };
                RecordedPoses.Add(newPair);
            }
        }
        if (applyPoses)
        {
            applyPoses = false;
            if(Objects.Count == 0){ Debug.Log("Object selection is empty."); return; }
            if(RecordedPoses.Count == 0){ Debug.Log("Recorded poses not found."); return; }
            if(RecordedPoses.Count < Objects.Count){ Debug.Log("Recorded poses incomplete."); return; }

            for(int i = 0; i < Objects.Count; i++)
            {
                Objects[i].position = RecordedPoses[i].Position;
                Objects[i].rotation = RecordedPoses[i].Rotation;
            }
        }
    }
}
