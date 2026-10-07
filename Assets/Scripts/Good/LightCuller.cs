using System;
using System.Collections.Generic;
using UnityEngine;

public class LightCuller : MonoBehaviour
{
    [Serializable]
    public class LightData
    {
        [SerializeField]
        public Light light;
        public bool wasOn;
    }

    public float cullDistance = 10f;
    public const float checkInterval = 1000f;
    public float checker = 0f;
    public List<LightData> Lights;
    public bool initialized = false;

    public void Initialize(float cullDistance)
    {
        this.cullDistance = cullDistance;
        if(!initialized)
        {
            ReQueryLights();
        }
        initialized = true;
    }
    void ReQueryLights()
    {
        GameObject[] roots = FindObjectsByType<GameObject>();

        foreach (GameObject root in roots)
        {
            root.TryGetComponent(out Light light);
            if (light)
            {
                int didaddlight = 0;
                foreach (LightData lightData in Lights)
                {
                    if (lightData.light == light)
                    {
                        didaddlight++;
                    }
                }
                if(didaddlight == 0)
                Lights.Add(new LightData { light = light, wasOn = light.enabled });
            }
        }
    }

    void FixedUpdate()
    {
        if(!initialized) return;

        if(cullDistance == 0f) return;

        checker++;
        
        if(checker > checkInterval)
        {
            checker = 0f;
            ReQueryLights();
        }
        else
        {
            foreach (LightData lightData in Lights)
            {
                if (Vector3.Distance(lightData.light.transform.position, transform.position) > cullDistance)
                {
                    lightData.light.enabled = false;
                }
                else
                {

                    lightData.light.enabled = true;
                }
            }
        }
    }
}
