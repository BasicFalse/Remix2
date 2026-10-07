using UnityEditor;
using UnityEngine;
using System;

public class DateToUnixWindow : EditorWindow
{
    private int year = 2025;
    private int month = 1;
    private int day = 1;
    private int hour = 0;
    private int minute = 0;
    private int second = 0;

    private long unixTime;

    private DateTime dateTime;

    private int UnixConv;

    [MenuItem("Tools/Time")]
    public static void ShowWindow()
    {
        GetWindow<DateToUnixWindow>("Unix Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Unix Time Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Date input fields
        EditorGUILayout.LabelField("Convert Type");
        UnixConv = EditorGUILayout.IntSlider(UnixConv,0,1);
        EditorGUILayout.Space();
        year = EditorGUILayout.IntField("Year", year);
        month = Mathf.Clamp(EditorGUILayout.IntField("Month", month), 1, 12);
        day = Mathf.Clamp(EditorGUILayout.IntField("Day", day), 1, 31);
        hour = Mathf.Clamp(EditorGUILayout.IntField("Hour", hour), 0, 23);
        minute = Mathf.Clamp(EditorGUILayout.IntField("Minute", minute), 0, 59);
        second = Mathf.Clamp(EditorGUILayout.IntField("Second", second), 0, 59);
        unixTime = EditorGUILayout.LongField("Unix", unixTime);

        EditorGUILayout.Space();

        if (UnixConv == 0)
        {
            try
            {
                dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
                unixTime = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Invalid date/time: {ex.Message}");
            }
        }
        else
        {
            dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
            year = dateTime.Year;
            month = dateTime.Month;
            day = dateTime.Day;
            hour = dateTime.Hour;
            minute = dateTime.Minute;
            second = dateTime.Second;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Unix Time", unixTime.ToString());
        EditorGUILayout.LabelField("DateTime", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
