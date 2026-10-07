using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ObjectsOnSpline : MonoBehaviour
{
    public SplineContainer spline;
    public GameObject prefab;
    public int count = 10;

    [Header("Spline Selection")]
    public bool useAllSplines = false;
    public int splineIndex = 0;

    [Header("Options")]
    public bool alignToSpline = true;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public List<GameObject> pool = new List<GameObject>();

    [ContextMenu("Generate Objects")]
    public void Generate()
    {
        if (spline == null || prefab == null)
            return;

        if (useAllSplines)
            GenerateAcrossAll();
        else
            GenerateSingle();
    }

    void GenerateSingle()
    {
        if (splineIndex < 0 || splineIndex >= spline.Splines.Count)
            return;

        var s = spline.Splines[splineIndex];

        EnsurePool(count);

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : i / (float)(count - 1);
            PlaceObject(pool[i], s, t);
        }

        CleanupPool();
    }

    void GenerateAcrossAll()
    {
        int splineCount = spline.Splines.Count;
        if (splineCount == 0) return;

        EnsurePool(count);

        for (int i = 0; i < count; i++)
        {
            float globalT = (count == 1) ? 0f : i / (float)(count - 1);

            float scaled = globalT * splineCount;
            int index = Mathf.Min(Mathf.FloorToInt(scaled), splineCount - 1);
            float localT = scaled - index;

            var s = spline.Splines[index];

            PlaceObject(pool[i], s, localT);
        }

        CleanupPool();
    }

    void PlaceObject(GameObject obj, Spline s, float t)
    {
        obj.SetActive(true);

        Vector3 localPos = s.EvaluatePosition(t);
        Vector3 pos = spline.transform.TransformPoint(localPos);

        Vector3 localTan = s.EvaluateTangent(t);
        Vector3 tan = spline.transform.TransformDirection(localTan);

        obj.transform.position = pos;

        if (alignToSpline && tan != Vector3.zero)
            obj.transform.rotation = Quaternion.LookRotation(tan);
        else
            obj.transform.rotation = Quaternion.identity;

        obj.transform.position += obj.transform.TransformDirection(positionOffset);
        obj.transform.rotation *= Quaternion.Euler(rotationOffset);
    }

    void EnsurePool(int targetCount)
    {
        while (pool.Count < targetCount)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.name = prefab.name + "_" + pool.Count;
            pool.Add(obj);
        }
    }

    void CleanupPool()
    {
        for (int i = pool.Count - 1; i >= count; i--)
        {
            DestroyImmediate(pool[i]);
            pool.RemoveAt(i);
        }
    }
}
