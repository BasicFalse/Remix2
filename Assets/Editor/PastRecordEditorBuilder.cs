#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PastRecordRebuilderWindow : EditorWindow
{
    private TextAsset jsonFile; // drag & drop a JSON here
    private PastRecord targetRecord;
    private string defaultAssetPath = "Assets/Records/PastRecord_Rebuilt.asset";

    [MenuItem("Tools/PastRecord/Rebuild From JSON (Window)")]
    public static void Open()
    {
        GetWindow<PastRecordRebuilderWindow>("PastRecord Rebuilder");
    }

    private void OnGUI()
    {
        GUILayout.Label("PastRecord JSON Rebuilder", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        jsonFile = (TextAsset)EditorGUILayout.ObjectField(
            "JSON File",
            jsonFile,
            typeof(TextAsset),
            false
        );

        targetRecord = (PastRecord)EditorGUILayout.ObjectField(
            "Target PastRecord (Optional)",
            targetRecord,
            typeof(PastRecord),
            false
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild"))
        {
            RebuildFromJson();
        }
    }

    private void RebuildFromJson()
    {
        if (jsonFile == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a JSON file.", "OK");
            return;
        }

        string jsonText = jsonFile.text;
        if (string.IsNullOrEmpty(jsonText))
        {
            EditorUtility.DisplayDialog("Error", "JSON file is empty.", "OK");
            return;
        }

        // Deserialize into a wrapper class
        PastRecordWrapper wrapper = null;
        try
        {
            wrapper = JsonUtility.FromJson<PastRecordWrapper>(jsonText);
        }
        catch
        {
            Debug.LogError("Failed to parse JSON. Make sure it matches the PastRecord structure.");
            return;
        }

        if (wrapper == null || wrapper.Record == null)
        {
            Debug.LogError("JSON did not contain valid Record data.");
            return;
        }

        // Create a new PastRecord asset if none assigned
        if (targetRecord == null)
        {
            targetRecord = ScriptableObject.CreateInstance<PastRecord>();
            string path = AssetDatabase.GenerateUniqueAssetPath(defaultAssetPath);
            AssetDatabase.CreateAsset(targetRecord, path);
        }

        Undo.RecordObject(targetRecord, "Rebuild PastRecord");

        targetRecord.Record.Clear();

        // Convert the wrapper snapshots into PastRecord.Snapshot
        foreach (var snap in wrapper.Record)
        {
            var snapshot = new PastRecord.Snapshot();
            snapshot.Snap(
                snap.BodyPosition, snap.BodyRotation,
                snap.HeadPosition, snap.HeadRotation,
                snap.HandLPosition, snap.HandLRotation,
                snap.HandRPosition, snap.HandRRotation,
                snap.isFlashlightOn
            );
            targetRecord.Record.Add(snapshot);
        }

        EditorUtility.SetDirty(targetRecord);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = targetRecord;

        Debug.Log($"Rebuilt {targetRecord.Record.Count} snapshots from {jsonFile.name}");
    }

    // Helper wrapper class for JSON deserialization
    [System.Serializable]
    private class PastRecordWrapper
    {
        public List<SnapshotWrapper> Record;
    }

    [System.Serializable]
    private class SnapshotWrapper
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
    }
}
#endif
