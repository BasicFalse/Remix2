using UnityEngine;

public class WorldSpaceSlider : RemixScript
{
    public Transform handle;            // The physical handle that moves
    public Transform startPoint;        // The minimum position (0)
    public Transform endPoint;          // The maximum position (9)
    [Range(0, 9)] public float currentValue; // Current int value
    public bool debugLog = false;

    public float val;
    public float val2;


    private Vector3 localStart;
    private Vector3 localEnd;

    public override void StartExt()
    {
        localStart = startPoint.localPosition;
        localEnd = endPoint.localPosition;
        
    }

    void Update()
    {
        //handleRB.linearVelocity = Vector3.zero;
        Vector3 localPos = handle.localPosition;
        Vector3 clamped = ClampToLine(localPos, localStart, localEnd);
        handle.localPosition = clamped;

        // Calculate % along line
        float t = Vector3.Distance(localStart, clamped) / Vector3.Distance(localStart, localEnd);
        //Debug.Log("COLOR SLIDER : COLOR READING AS")

        val = t;

        // Map to 0–9
        int newValue = Mathf.RoundToInt(t * 9);

        val2 = newValue;

        if (newValue != currentValue)
        {
            currentValue = newValue;
            if (debugLog) Debug.Log(name + " slider = " + currentValue);
        }
    }

    Vector3 ClampToLine(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    public void SetValue(float value)
    {
        value = Mathf.Clamp(value, 0, 9);
        float t = value / 9f;
        handle.localPosition = Vector3.Lerp(localStart, localEnd, t);
        currentValue = value;
    }
}
