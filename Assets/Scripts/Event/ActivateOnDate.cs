using System;
using UFunction;
using UnityEngine;

public class ActivateOnDate : MonoBehaviour
{
    public bool eventTime;

    public bool lessThanOneWeek;
    public bool lessThanOneDay;
    public bool lessThanOneHour;

    public TimeStamp setTime;

    [Header("Event Date and Time (UTC)")]
    public int activationYear = 2023;
    public int activationMonth = 4;
    public int activationDay = 1;
    public int activationHour = 7;
    public int activationMinute;
    public int activationSecond;

    [Header("Event Calibration Date and Time (UTC)")]
    public int deactivationYear = 2023;
    public int deactivationMonth = 4;
    public int deactivationDay = 2;
    public int deactivationHour = 7;
    public int deactivationMinute;
    public int deactivationSecond;

    private DateTime activationTime;
    private DateTime deactivationTime;

    public double dbgTimeUntilActivation;
    public double dbgTimeUntilDeactivation;

    public string formattedActivation;
    public LiveEventManager LEM;

    private void OnEnable()
    {
        //Invoke("InitActiveTimes", 0.001f);
        if (setTime != null)
        {
            activationYear = setTime.year;
            activationMonth = setTime.month;
            activationDay = setTime.day;
            activationHour = setTime.hour;
            activationMinute = setTime.minute;
            activationSecond = setTime.second;
        }
        InitActiveTimes();
    }


    private void InitActiveTimes()
    {
        activationTime = new DateTime(activationYear, activationMonth, activationDay, activationHour, activationMinute, activationSecond, DateTimeKind.Utc);
        deactivationTime = new DateTime(deactivationYear, deactivationMonth, deactivationDay, deactivationHour, deactivationMinute, deactivationSecond, DateTimeKind.Utc);
    }

    private void LateUpdate()
    {
        DateTime utcNow = DateTime.UtcNow;
        dbgTimeUntilActivation = (activationTime - utcNow).TotalSeconds;
        dbgTimeUntilDeactivation = (deactivationTime - utcNow).TotalSeconds;

        if (dbgTimeUntilActivation <= 0 && dbgTimeUntilActivation >= -600)
        {
            eventTime = true;
        }
        else
        {
            eventTime = false;
        }

        LEM.canPlay = eventTime;
        FormatTime(dbgTimeUntilActivation);
    }

    public string FormatTime(double totalSeconds)
    {
        if (totalSeconds < -600)
            return "EventPassed";

        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
        int weeks = timeSpan.Days / 7;
        int days = timeSpan.Days % 7;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        string formattedTime = "";
        if (weeks > 0) formattedTime += weeks + (weeks == 1 ? " week " : " weeks ");
        if (days > 0) formattedTime += days + (days == 1 ? " day " : " days ");
        if (hours > 0) formattedTime += hours + (hours == 1 ? " hour " : " hours ");
        if (minutes > 0) formattedTime += minutes + (minutes == 1 ? " minute " : " minutes ");
        if (seconds > 0 || formattedTime == "") formattedTime += seconds + (seconds == 1 ? " second" : " seconds");

        if (weeks <= 0) lessThanOneWeek = true; else lessThanOneWeek = false;
        if (days <= 0 && weeks <= 0) lessThanOneDay = true; else lessThanOneDay = false;
        if (hours <= 0 && days <= 0 && weeks <= 0) lessThanOneHour = true; else lessThanOneHour = false;

        formattedActivation = formattedTime;
        return formattedTime.Trim();
    }
}
