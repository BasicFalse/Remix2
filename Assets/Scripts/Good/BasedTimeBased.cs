using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class BasedTimeBased : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds60 = new(60f);
    private const string TimeApi = "https://timeapi.io/api/time/current/zone?timeZone=UTC";

    public long UnixTimestampUtc  { get; private set; }
    public DateTime DateTimeUtc   { get; private set; }
    public long UnixTimestampLocal { get; private set; }
    public DateTime DateTimeLocal  { get; private set; }

    [Header("UTC (from API)")]
    public int year, month, day, hour, minute, second;

    [Header("Local (device)")]
    public int yearLocal, monthLocal, dayLocal, hourLocal, minuteLocal, secondLocal;

    [Header("--------------------")]
    public TimedEvents TimelyEvents;

    void Start() => StartCoroutine(AutoRefreshLoop());

    private IEnumerator AutoRefreshLoop()
    {
        while (true)
        {
            yield return StartCoroutine(GetAccurateTime());
            TimelyEvents.CheckTimedEvents(hourLocal);
            yield return _waitForSeconds60;
        }
    }

    private IEnumerator GetAccurateTime()
    {
        Debug.Log("[TimeService] Fetching time...");

        // --- Always capture local time first so it's never zero ---
        DateTimeLocal      = DateTime.Now;
        UnixTimestampLocal = ((DateTimeOffset)DateTimeLocal).ToUnixTimeSeconds();

        using (UnityWebRequest www = UnityWebRequest.Get(TimeApi))
        {
            www.timeout = 5;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    long onlineUnix   = ExtractUnixTime(www.downloadHandler.text);
                    UnixTimestampUtc  = onlineUnix;
                    DateTimeUtc       = DateTimeOffset.FromUnixTimeSeconds(onlineUnix).UtcDateTime;
                    Debug.Log($"[TimeService] Online UTC unix: {UnixTimestampUtc}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[TimeService] JSON parse failed, using local time. ({e.Message})");
                    UnixTimestampUtc = UnixTimestampLocal;
                    DateTimeUtc      = DateTimeLocal.ToUniversalTime();
                }
            }
            else
            {
                Debug.LogWarning($"[TimeService] Offline — using local time. ({www.error})");
                UnixTimestampUtc = UnixTimestampLocal;
                DateTimeUtc      = DateTimeLocal.ToUniversalTime();
            }
        }

        Debug.Log($"[TimeService] Local unix: {UnixTimestampLocal}");  // was printing wrong var before
        ExtractFields();
    }

    private void ExtractFields()
    {
        // UTC fields
        year   = DateTimeUtc.Year;   month   = DateTimeUtc.Month;
        day    = DateTimeUtc.Day;    hour    = DateTimeUtc.Hour;
        minute = DateTimeUtc.Minute; second  = DateTimeUtc.Second;

        // Local fields
        yearLocal   = DateTimeLocal.Year;   monthLocal   = DateTimeLocal.Month;
        dayLocal    = DateTimeLocal.Day;    hourLocal    = DateTimeLocal.Hour;
        minuteLocal = DateTimeLocal.Minute; secondLocal  = DateTimeLocal.Second;
    }

    private long ExtractUnixTime(string json)
    {
        const string tag = "\"unixtime\":";
        int index = json.IndexOf(tag);
        if (index < 0) throw new Exception("No 'unixtime' field in JSON");

        index += tag.Length;
        int end = index;
        while (end < json.Length && char.IsDigit(json[end])) end++;

        return long.Parse(json.Substring(index, end - index));
    }
}