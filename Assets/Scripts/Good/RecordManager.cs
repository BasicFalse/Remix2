using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using GorillaRigging;

public class RecordManager : MonoBehaviour
{
    public enum RecordButtonSetting { Record, Stop, Play, RecordAndPlay }

    [System.Serializable]
    public class RecordAndRigPair
    {
        public PastRecord record;
        public RecordRig rig;
    }

    public bool isRecording = false;
    public bool isPlaying = false;

    public int currentSnapshotIndex = 0;
    public GameObject recordedRigPrefab;

    [SerializeField]
    public List<RecordAndRigPair> recordingPairs = new();

    public RecordAndRigPair recordingPair;

    public RigController PlayerRig;
    public PlayerBase Player;

    void FixedUpdate()
    {
        if (isRecording)
        {
            recordingPair.record.CreateSnapShot(
                PlayerRig.OfflineBody.position,
                PlayerRig.OfflineBody.rotation,
                Player.transforms.Camera.transform.position,
                Player.transforms.Camera.transform.rotation,
                Player.transforms.LeftController.transform.position,
                Player.transforms.LeftController.transform.rotation,
                Player.transforms.RightController.transform.position,
                Player.transforms.RightController.transform.rotation,
                Player.bools.isFlashlightOn
            );
        }

        if(isPlaying)
        {
            int pairsDone = 0;
            currentSnapshotIndex++;
            foreach (var pair in recordingPairs)
            {

                PastRecord currentRecord = pair.record;
                if (currentSnapshotIndex < currentRecord.Record.Count)
                {
                    PastRecord.Snapshot snapshot = currentRecord.Record[currentSnapshotIndex];

                    //RecordRig.bodyLink.SetNetworkTarget(snapshot.BodyPosition, snapshot.BodyRotation, RecordRig.networkInterpTime);
                    pair.rig.Body.SetPositionAndRotation(snapshot.BodyPosition, snapshot.BodyRotation);
                    pair.rig.Head.SetPositionAndRotation(snapshot.HeadPosition, snapshot.HeadRotation);
                    pair.rig.LHand.SetPositionAndRotation(snapshot.HandLPosition, snapshot.HandLRotation);
                    pair.rig.RHand.SetPositionAndRotation(snapshot.HandRPosition, snapshot.HandRRotation);
                    pair.rig.isFlashlightOn = snapshot.isFlashlightOn;

                }
                else
                {
                    pairsDone++;
                    Debug.Log("Playback Finished");
                }

                if(pairsDone >= recordingPairs.Count)
                {
                    isPlaying = false;
                    currentSnapshotIndex = 0;
                    Debug.Log("All Recordings Finished Playing");
                }
            }
        }
    }

    public RecordAndRigPair CreateNewRecordingPair()
    {
        RecordAndRigPair newPair = new()
        {
            record = ScriptableObject.CreateInstance<PastRecord>(),
            rig = Instantiate(recordedRigPrefab).GetComponent<RecordRig>()
        };
        //recordingPairs.Add(newPair);
        currentSnapshotIndex = 0;

        return newPair;
    }

    public void ResetRecording(PastRecord record)
    {
        record.Record.Clear();
        currentSnapshotIndex = 0;
    }

    public void StartRecording()
    {
        isRecording = true;
        isPlaying = false;
        currentSnapshotIndex = 0;
        recordingPair = CreateNewRecordingPair();
        Debug.Log("Recording Started");
    }

    public void StopRecording()
    {
        isRecording = false;
        isPlaying = false;
        currentSnapshotIndex = 0;
        PastRecordJsonIO.Save(recordingPair.record);
        recordingPairs.Add(recordingPair);
        Debug.Log("Recording Stopped");
    }

    public void PlayRecording()
    {
        isRecording = false;
        isPlaying = true;
        currentSnapshotIndex = 0;
        Debug.Log("Playing Recording");
    }

    public void StartRecordingAndPlaying()
    {
        isRecording = true;
        isPlaying = true;
        currentSnapshotIndex = 0;
        recordingPair = CreateNewRecordingPair();
        Debug.Log("Recording and Playing Started");
    }
}
