using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGeoController : MonoBehaviour
{
    
    public enum BGOS{ Aboveground, Underground, AboveAndBelowground, Disabled, MapSpecific, MapSpecificInverted}
    [System.Serializable]
    public class Map
    {
        [SerializeField]
        public string mapName;
        public bool usesSceneAsset;
        public bool isSceneLoaded;
        public bool isUnderground;
        public GameObject[] Objects;
        public SceneReference MapScene;
        public GameObject[] MapCovers;
        public List<string> associatedMaps;
        public bool isStartingMap;
    }
    [System.Serializable]
    public class ConditionalBGO
    {
        [SerializeField]
        public GameObject BGObject;
        public BGOS groundStatus;
        public List<string> accMaps;
    }
    [System.Serializable]
    public class OptimizedPaths
    {
        [SerializeField]
        public GameObject pathGO;
        public List<string> linkingMaps;
    }



    public static volatile MapGeoController instance;
    public MiscGrabs MG;
    public List<ConditionalBGO> backgroundObjects;
    public List<OptimizedPaths> pathLinking;
    public List<string> currentMaps;
    public List<string> lastMaps;
    public bool freeze;
    public bool disableTriggerRenderers;
    public bool playerIsUnderground;
    public bool playerIsAboveground;
    public Material TriggerMaterial;
    public List<Map> Maps;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        instance = null;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(base.gameObject);
        }
        
#if !UNITY_EDITOR
        freeze = false;
#endif

        currentMaps.Clear();

        foreach (Map m in Maps)
        {
            if (m.isStartingMap)
            {
                currentMaps.Add(m.mapName);
            }
        }

        currentMaps = MG.SM.CurrentSpawnMaps(currentMaps.ToArray()).ToList();
        Debug.Log("current maps are loaded from spawn" );
        RedoGeo();
    }

    private void Update()
    {
        if (lastMaps != currentMaps)
        {
            RedoGeo();
            lastMaps = new List<string>(currentMaps);
        }
    }


    private void RedoGeo()
    {
        if (freeze) return;
        UpdateMaps();
        UpdateBackground();
        UpdatePaths();
    }

    private void UpdateMaps()
    {
        HashSet<string> mapsToActivate = new(currentMaps);
        int groundTestA = 0;
        int groundTestU = 0;

        foreach (Map m in Maps)
        {
            if (currentMaps.Contains(m.mapName))
            {
                if (m.isUnderground) { groundTestU++; } else { groundTestA++; }
                foreach (string assoc in m.associatedMaps)
                {
                    mapsToActivate.Add(assoc);
                }
            }
        }

        foreach (Map m in Maps)
        {
            bool shouldBeActive = mapsToActivate.Contains(m.mapName);

            if (shouldBeActive)
            {
                if (m.usesSceneAsset){StartCoroutine(LoadMapAsync(m));}
                else{LoadMap(m);}
            }
            else{UnloadMap(m);}
        }

        // hi-tek
        if (groundTestU > 0) { playerIsUnderground = true; } else { playerIsUnderground = false; }
        if (groundTestA > 0) { playerIsAboveground = true; } else { playerIsAboveground = false; }
    }

    private void UpdateBackground()
    {
        foreach (ConditionalBGO bgo in backgroundObjects)
        {
            switch (bgo.groundStatus)
            {
                case BGOS.Aboveground:
                    bgo.BGObject.SetActive(playerIsAboveground);
                    break;
                case BGOS.Underground:
                    bgo.BGObject.SetActive(playerIsUnderground);
                    break;
                case BGOS.AboveAndBelowground:
                    if (playerIsAboveground || playerIsUnderground) { bgo.BGObject.SetActive(true); } else { bgo.BGObject.SetActive(false); }
                    break;
                case BGOS.Disabled:
                    bgo.BGObject.SetActive(false);
                    break;
                case BGOS.MapSpecific:
                    int x = 0;
                    foreach (string s in bgo.accMaps) { if (currentMaps.Contains(s)) { x++; } }
                    if (x > 0)
                    { bgo.BGObject.SetActive(true); }
                    else { bgo.BGObject.SetActive(false); }
                    break;
                case BGOS.MapSpecificInverted:
                    int y = 0;
                    foreach (string s in bgo.accMaps) { if (currentMaps.Contains(s)) { y++; } }
                    if (y > 0)
                    { bgo.BGObject.SetActive(false); }
                    else { bgo.BGObject.SetActive(true); }
                    break;
            }
        }
    }

    private void UpdatePaths()
    {
        foreach (OptimizedPaths op in pathLinking)
        {
            int x = 0;
            foreach (string s in op.linkingMaps)
            {
                if (currentMaps.Contains(s)) { x++; }

            }
            if(x>0){op.pathGO.SetActive(true);}else{ op.pathGO.SetActive(false);}
        }
    }

    public void InitializeSpawn()
    {
        currentMaps = MG.SM.CurrentSpawnMaps(currentMaps.ToArray()).ToList();
        Debug.Log("current maps are loaded from spawn" );
        RedoGeo();
    }

    public void ModifyCurrentMaps(List<MapTrigger.MapTriggerSegment> desMap)
    {
        if (freeze) { return; }

        foreach (MapTrigger.MapTriggerSegment mts in desMap)
        {
            switch (mts.Function)
            {
                case MapTrigger.MapFunction.Add:
                    if (!currentMaps.Contains(mts.mapName))
                    {
                        currentMaps.Add(mts.mapName);
                    }
                    break;
                case MapTrigger.MapFunction.Remove:
                    currentMaps.Remove(mts.mapName);
                    break;
                case MapTrigger.MapFunction.Reset:
                    currentMaps.Clear();
                    currentMaps.Add(mts.mapName);
                    break;
                case MapTrigger.MapFunction.Null:
                    currentMaps.Clear();
                    break;
            }
        }

    }

    public void LoadMap(Map map)
    {
        for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
        {
            if (map.Objects.Length > i)
            {
                map.Objects[i].SetActive(true);
            }
            if (map.MapCovers.Length > i)
            {
                map.MapCovers[i].SetActive(false);
            }
        }

        for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
        {
            if (map.Objects.Length > i)
            {
                map.Objects[i].SetActive(true);
            }
            if (map.MapCovers.Length > i)
            {
                map.MapCovers[i].SetActive(false);
            }
        }
    }

    IEnumerator LoadMapAsync(Map map)
    {
        if(!map.isSceneLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(map.MapScene, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = true;

            map.isSceneLoaded = true;

            while (!loadOperation.isDone)
            {    
                yield return null;
            }

            for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
            {
                if (map.Objects.Length > i)
                {
                    map.Objects[i].SetActive(true);
                }
                if (map.MapCovers.Length > i)
                {
                    map.MapCovers[i].SetActive(false);
                }
            }

            for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
            {
                if (map.Objects.Length > i)
                {
                    map.Objects[i].SetActive(true);
                }
                if (map.MapCovers.Length > i)
                {
                    map.MapCovers[i].SetActive(false);
                }
            }
        }

        
    }

    public void UnloadMap(Map map)
    {
        if(map.isSceneLoaded){
        SceneManager.UnloadSceneAsync(map.MapScene);
        map.isSceneLoaded = false;
        }

        for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
        {
            if (map.Objects.Length > i)
            {
                map.Objects[i].SetActive(false);
            }
            if (map.MapCovers.Length > i)
            {
                map.MapCovers[i].SetActive(true);
            }
        }

        for (int i = 0; i < map.Objects.Length || i < map.MapCovers.Length; i++)
        {
            if (map.Objects.Length > i)
            {
                map.Objects[i].SetActive(false);
            }
            if (map.MapCovers.Length > i)
            {
                map.MapCovers[i].SetActive(true);
            }
        }
    }

    public SpawnManager.SpawnDat ErrorRoomSequence()
    {
        SpawnManager.SpawnDat errorSpawnCoords = new();

        foreach (Map m in Maps)
        {
            if (m.mapName == "ErrorRoom")
            {
                errorSpawnCoords.position = m.Objects[1].transform.position;
                errorSpawnCoords.eulerAngles = m.Objects[1].transform.eulerAngles;
                PlayerBase.i.resources.RB.transform.parent.SetPositionAndRotation
                (m.Objects[1].transform.position, m.Objects[1].transform.rotation);
                PlayerBase.i.GorillaVibration.transform.localPosition = Vector3.zero;
            }
        }

        MiscGrabs.grab.SkyManager.DisableSkybox();

        PlayerBase.i.GorillaVibration.transform.localPosition = Vector3.zero;
        
        currentMaps.Clear();
        currentMaps.Add("ErrorRoom");
        RedoGeo();

        PlayerBase.i.GorillaVibration.transform.localPosition = Vector3.zero;

        return errorSpawnCoords;
    }
}
