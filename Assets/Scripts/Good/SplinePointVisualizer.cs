using SplineTools;
using UnityEngine;

public class SplinePointVisualizer : MonoBehaviour
{
    public GameObject go;
    public Zipline s;

    void Update()
    {
        go.transform.position = SplineEvaluation.GetWorldPosition(s.splineContainer, s.t);
    }
}
