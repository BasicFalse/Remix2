using System.Collections.Generic;
using GorillaLocomotion.Swimming;
using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    public bool InWater;

    public List<WaterVolume> colliders;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<WaterVolume>()){
            colliders.Add(other.GetComponent<WaterVolume>());
            InWater = colliders.Count > 0;}
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<WaterVolume>()){
            colliders.Remove(other.GetComponent<WaterVolume>());
            InWater = colliders.Count > 0;}
    }
}
