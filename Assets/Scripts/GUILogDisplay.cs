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
        for (int i = 0; i < LoggingStatics.noLogers.Length; i++)
        {
            if(s == LoggingStatics.noLogers[i])
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
