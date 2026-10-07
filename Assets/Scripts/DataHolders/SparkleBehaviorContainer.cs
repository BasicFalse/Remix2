using UnityEngine;

[CreateAssetMenu(fileName = "SparkleBehaviorContainer", menuName = "DataHolders/SparkleBehaviorContainer")]
public class SparkleBehaviorContainer : ScriptableObject
{   
    public float maxSpeed = 5f;
    public float steeringForce = 5f;
    public float neighborRadius = 2.5f;
    public float separationRadius = 1f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 2f;
    public float targetAttractionWeight = 3f;
}
