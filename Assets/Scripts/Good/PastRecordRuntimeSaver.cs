using System;
using System.IO;
using UnityEngine;

public static class PastRecordJsonIO
{
    // Returns a platform-safe path
    static string GetSavePath()
    {
        // persistentDataPath is writable on all platforms, including Android
        string folder = Application.persistentDataPath;

        // Optional: create folder if it doesn't exist
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Add timestamped filename
        string filename = "PastRecord_Runtime_" + UnityEngine.Time.time.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";

        return Path.Combine(folder, filename);
    }

    public static void Save(PastRecord record)
    {
        if(record == null || record.Record == null)
        {
            Debug.Log("Cannot save null PastRecord.");
            return;
        }
        PastRecord save = new()
        {
            Record = record.Record
        };

        string json = JsonUtility.ToJson(save, true);
        string path = GetSavePath();

        File.WriteAllText(path, json);
        Debug.Log($"PastRecord JSON saved:\n{path}");
    }

    // Optionally load from a specific file path
    public static bool Load(string path, out PastRecord data)
    {
        if (!File.Exists(path))
        {
            data = null;
            return false;
        }

        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<PastRecord>(json);
        return true;
    }

    // Overload to automatically load the latest file
    public static bool LoadLatest(out PastRecord data)
    {
        string folder = Application.persistentDataPath;
        var files = Directory.GetFiles(folder, "PastRecord_Runtime_*.json");

        if (files.Length == 0)
        {
            data = null;
            return false;
        }

        // Sort by creation time, latest first
        Array.Sort(files, (a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));

        return Load(files[0], out data);
    }
}
