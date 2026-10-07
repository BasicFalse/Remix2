using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateRawFile
{
    [MenuItem("Assets/Create/Txt File", priority = 81)]
    public static void CreateTextFileAsset()
    {
        string folderPath = GetCurrentFolderPath();
        string fullPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, "NewTextFile.txt")
        );

        File.WriteAllText(fullPath, "");
        AssetDatabase.Refresh();

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }

    [MenuItem("Assets/Create/Json File", priority = 81)]
    public static void CreateJsonFileAsset()
    {
        string folderPath = GetCurrentFolderPath();
        string fullPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, "NewJsonFile.json")
        );

        File.WriteAllText(fullPath, "");
        AssetDatabase.Refresh();

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }

    private static string GetCurrentFolderPath()
    {
        string path = "Assets";

        if(Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }

        return path;
    }
    
}
