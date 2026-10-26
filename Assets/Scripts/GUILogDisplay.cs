using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RemixLogging;

public class GUILogDisplay : MonoBehaviour
{

    public TextMeshPro outText;
    List<LogDisplay> Logs = new();
    const float permittedLifetime = 5f;
    private static readonly string[] noLogers = new string[]
    {
        "Audio source failed to initialize audio spatializer. An audio spatializer is specified in the audio project settings, but the associated plugin was not found or initialized properly. Please make sure that the selected spatializer is compatible with the target.",
        "dll file in: Not a directory",
        "movement check failed! we had to use the backup raycast. investigate the conditions that caused this to happen",
        "Look rotation viewing vector is zero"
    };
    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }
    public void Log(string logString, string stackTrace, LogType type)
    {
        if(AnyMatch(logString)) return;

        LogDisplay nld = new() { content = logString, stack = "", lifetime = 0f, severity = LoggingConversion.LogTypeToInt(type) };

        Logs.Add(nld);
    }
    private bool AnyMatch(string s)
    {
        foreach(string s2 in noLogers)
        {
            if(s == s2)
            {
                return true;
            }
        }
        return false;
    }
    void Update()
    {
        outText.text = "";
        for(int i = 0; i<Logs.Count; i++)
        {
            LogDisplay log = Logs[i];
            log.lifetime += Time.deltaTime;
            Logs[i] = log;
            if(log.lifetime > permittedLifetime)
            {
                Logs.Remove(log);
                i--;
                continue;
            }
            outText.text += LoggingConversion.IntToRichStringColor(Logs[i].severity) + Logs[i].content + "</color>\n";
        }
    }
}
