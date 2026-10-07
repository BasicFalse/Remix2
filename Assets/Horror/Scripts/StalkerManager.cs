using UnityEngine;
using System.Collections.Generic;

public class StalkerManager : MonoBehaviour
{
    public GameObject stalkerPrefab;
    public List<StalkerSpawn> spawnLocations = new();
    public float spawnCooldown = 10f;

    private float nextSpawnTime = 0f;

    private GameObject currentStalker;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnCooldown;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && currentStalker == null)
        {
            SpawnStalker();
        }
    }

    private void SpawnStalker()
    {
        if (spawnLocations.Count == 0)
        {
            Debug.LogError("No spawn locations assigned.");
            return;
        }

        // Pick a random spawn location
        int randomIndex = Random.Range(0, spawnLocations.Count);
        StalkerSpawn spawnLocation = spawnLocations[randomIndex];

        

        if(spawnLocation.spawnMap.Length == 0)
        {
            currentStalker = Instantiate(stalkerPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
        }
        else
        {
            int i =0;
            
            foreach(string s in spawnLocation.spawnMap)
            {
                if (MapGeoController.instance.currentMaps.Contains(s))
                {
                    i++;
                }
            }

            if (i < 0)
            {
                currentStalker = Instantiate(stalkerPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
            }
        }

        // Set the next allowed spawn time
        nextSpawnTime = Time.time + spawnCooldown;
    }

    public void DestroyCurrentStalker()
    {
        if (currentStalker != null)
        {
            Destroy(currentStalker);
            currentStalker = null;
        }
    }
}