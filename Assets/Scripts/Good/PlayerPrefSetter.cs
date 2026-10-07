using UnityEngine;

public class PlayerPrefSetter : MonoBehaviour
{
    public string key;
    public string value;

    void OnTriggerEnter(Collider other)
    {
        PlayerPrefs.SetString(key,value);
        PlayerPrefs.Save();
    }
}
