using UnityEngine;
using System;
using System.Collections;

public class TimeFetcher : RemixScript
{
    public string timr;

    public DateTime timraw;

    public override void StartExt()
    {
        StartCoroutine(FetchDateTimeEveryFiveSeconds());
    }

    IEnumerator FetchDateTimeEveryFiveSeconds()
    {
        while (true)
        {
            // Get current date and time
            DateTime currentDateTime = DateTime.Now;
            timr = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            timraw = currentDateTime;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
