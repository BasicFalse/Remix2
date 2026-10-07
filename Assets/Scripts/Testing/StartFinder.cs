using UnityEngine;

public class StartFinder : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log(JsonFileNavigator.GetString(Application.streamingAssetsPath + "/U-TextFinder/TestFile.json", "test"));
    }
}
