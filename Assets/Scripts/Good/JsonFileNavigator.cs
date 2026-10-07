using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class JsonFileNavigator
{
    public static string GetString(string fileName, string key)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = File.ReadAllText(path);
        JObject obj = JObject.Parse(json);

        string data = obj[key].ToString();
        
        return data;
    }

    public static string GetString(string fileName, string key, string subKey)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = File.ReadAllText(path);
        JObject obj = JObject.Parse(json);

        string data = obj[key].ToString();
        
        return data;
    }
}
