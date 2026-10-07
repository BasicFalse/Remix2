using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PastRecord", menuName = "General/PastRecord")]
public class PastRecord : ScriptableObject
{
    [System.Serializable]
    public class Snapshot
    {
        public Vector3 BodyPosition;
        public Quaternion BodyRotation;
        public Vector3 HeadPosition;
        public Quaternion HeadRotation;
        public Vector3 HandLPosition;
        public Quaternion HandLRotation;
        public Vector3 HandRPosition;
        public Quaternion HandRRotation;

        public bool isFlashlightOn;

        public void Snap(Vector3 BodyPosition, Quaternion BodyRotation, Vector3 HeadPosition, Quaternion HeadRotation, Vector3 HandLPosition, Quaternion HandLRotation, Vector3 HandRPosition, Quaternion HandRRotation, bool flashlightStatus)
        {
            this.BodyPosition = BodyPosition;
            this.BodyRotation = BodyRotation;
            this.HeadPosition = HeadPosition;
            this.HeadRotation = HeadRotation;
            this.HandLPosition = HandLPosition;
            this.HandLRotation = HandLRotation;
            this.HandRPosition = HandRPosition;
            this.HandRRotation = HandRRotation;
            isFlashlightOn = flashlightStatus;
        }
    }

        
        public List<Snapshot> Record = new();

        public void AddSnapshot(Snapshot snapshot)
        {
            Record.Add(snapshot);
        }

        public void CreateSnapShot(Vector3 BodyPosition, Quaternion BodyRotation, Vector3 HeadPosition, Quaternion HeadRotation, Vector3 HandLPosition, Quaternion HandLRotation, Vector3 HandRPosition, Quaternion HandRRotation, bool flashlightStatus)
        {
            Snapshot snapshot = new();
            snapshot.Snap(BodyPosition, BodyRotation, HeadPosition, HeadRotation, HandLPosition, HandLRotation, HandRPosition, HandRRotation, flashlightStatus);
            AddSnapshot(snapshot);
        }
        
}
