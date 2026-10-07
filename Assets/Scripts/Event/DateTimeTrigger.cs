using UnityEngine;
using System;

public class DateTimeTrigger : MonoBehaviour
{
    [SerializeField] private int year = 2025;
    [SerializeField] private int month = 3;
    [SerializeField] private int day = 21;
    [SerializeField] private int hour = 1;
    [SerializeField] private int minute = 0;
    [SerializeField] private int second = 0;

    public bool isTriggered = false;

    private void Update()
    {
        DateTime targetDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        if (DateTime.UtcNow >= targetDate && !isTriggered)
        {
            isTriggered = true;
            Debug.Log("Target date and time reached (UTC)!");
        }
    }
}
