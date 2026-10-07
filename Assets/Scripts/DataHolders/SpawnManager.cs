using UnityEngine;

[CreateAssetMenu(fileName = "SpawnManager", menuName = "System/SpawnManager")]
public class SpawnManager : ScriptableObject
{
    [System.Serializable]
    public class SpawnDat
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public bool disableFlashlight;
        //public bool trueAfterSpawn;
        public bool changeMaps;
        public string[] maps;
    }

    [System.Serializable]
    public class SpawnListSegment
    {
        public string link;
        public SpawnDat sd;
    }
    public string SpawnMode;
    public string InitialCheck;
    public SpawnDat ErrorRoom;
    public SpawnDat BasicSpawn;
    public SpawnDat[] TutorialSpawns;
    public SpawnListSegment[] TutorialMatches;
    public SpawnListSegment[] SpawnMatch;

    public bool DidDoTutorial()
    {
        bool didtutorial;
        string ppts = PlayerPrefs.GetString(InitialCheck);
        Debug.Log("tutcheck recalled as " + ppts + " from " + InitialCheck);
        switch (ppts)
        {
            case "true":
                didtutorial = true;
                break;
            default:
                PlayerPrefs.SetString(InitialCheck, "false");
                didtutorial = false;
                return didtutorial;
        }

        return didtutorial;
    }

    public SpawnDat CurrentSpawnDat()
    {
        if (DidDoTutorial() || InitialCheck == null || InitialCheck == "")
        {
            foreach (SpawnListSegment sls in SpawnMatch)
            {
                if(sls.link == SpawnMode)
                {
                    Debug.Log("DidntRanToErrorRoom");
                    return sls.sd;
                }
            }
        }

        //if (InitialCheck == "resonator")
        //{
        //    Debug.Log("DidntRanToErrorRoom");
        //    return RandomTutorialSelector();
        //}
        //if (InitialCheck == "didtutorial")
        //{
        //    PlayerPrefs.SetString("didtutorial", "true");
        //    return BasicSpawn;
        //}

        foreach (SpawnListSegment sls in TutorialMatches)
        {
            if(sls.link == InitialCheck)
            {
                Debug.Log("DidntRanToErrorRoom");
                return sls.sd;
            }
        }

        if(InitialCheck == "random")
        {
            return RandomTutorialSelector();
        }

        Debug.Log("RanToErrorRoom");
        ErrorRoomFalacy();
        PlayerPrefs.SetString(InitialCheck, "false");
        return ErrorRoom;
    }

    public string[] CurrentSpawnMaps(string[] currentMaps)
    {
        SpawnDat nsd = CurrentSpawnDat();
        if (nsd.changeMaps)
        {
            return nsd.maps;
        }
        return currentMaps;
    }
    
    public SpawnDat ErrorRoomFalacy()
    {
        //ErrorRoom = MapGeoController.instance.ErrorRoomSequence();
        MapGeoController.instance.ErrorRoomSequence();
        GorillaVibration.Instance.transform.localPosition = Vector3.zero;
        //Debug.Log("SPAWNMANAGER : INITIAL CHECK FAULT ; PLAYERPREF NOT ACCOUNTED FOR (" + InitialCheck + "), ERROR ROOM ACTIVE");
        //Debug.Log(ErrorRoom.position);
        return ErrorRoom;
    }

    public SpawnDat RandomTutorialSelector()
    {
        int randomIndex = Random.Range(0, TutorialSpawns.Length);
        return TutorialSpawns[randomIndex];
    }
}
