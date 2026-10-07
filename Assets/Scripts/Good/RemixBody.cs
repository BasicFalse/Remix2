using System.Collections.Generic;
using GorillaLocomotion.Swimming;
using GravNew;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RemixBody : NormalisedGravity
{
    List<Collider> currentWaterVolumes = new();
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<WaterVolume>())
        currentWaterVolumes.Add(other);
    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<WaterVolume>())
        currentWaterVolumes.Remove(other);
    }

    void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;

        if(swimmer.bodyWater.InWater){
            rb.useGravity = false;
            
        Vector3 zero = rb.linearVelocity;
		Vector3 zero2 = Vector3.zero;
        Vector3 velocity = rb.linearVelocity;
        List<WaterVolume> activeWaterVolumes = swimmer.bodyWater.colliders;
        List<WaterCurrent> activeWaterCurrents = new();
        for (int i = 0; i < activeWaterVolumes.Count; i++)
        {
            zero += activeWaterVolumes[i].naturalCurrent;
            if (activeWaterVolumes[i].waterCurrent)
            {
                activeWaterCurrents.Add(activeWaterVolumes[i].waterCurrent);
            }
        }
        for(int i =0; i < activeWaterCurrents.Count;i++)
        {if(!activeWaterCurrents[i]){activeWaterCurrents.Remove(activeWaterCurrents[i]);i--;}}
		for (int j = 0; j < activeWaterCurrents.Count; j++)
		{
			if (activeWaterCurrents[j].GetCurrentAtPoint
                (
                    startingVelocity: velocity + zero, 
                    worldPoint: transform.position, 
                    dt: fixedDeltaTime, 
                    currentVelocity: out var vector3, 
                    velocityChange: out var velocityChange))
			{
				zero2 += vector3;
				zero += velocityChange;
			}
		}
        rb.linearVelocity = zero;
    }
}
}