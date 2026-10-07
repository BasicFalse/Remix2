using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class LayerConverter : EditorWindow
{
    private bool UseSpecificParentObject = false;
    private GameObject ParentObject = null;
    private LayerMask FromLayer;
    private LayerMask ToLayer;
    

    [MenuItem("Tools/Layering")]
    public static void ShowWindow()
    {
        GetWindow<LayerConverter>("Layer Converter");
    }

    private void OnGUI()
    {
        UseSpecificParentObject = EditorGUILayout.Toggle("Use Specific Parent Object", UseSpecificParentObject);
        if(UseSpecificParentObject){
        ParentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", ParentObject, typeof(GameObject), true);}

        FromLayer = EditorGUILayout.LayerField("From Layer", FromLayer);
        ToLayer = EditorGUILayout.LayerField("To Layer", ToLayer);

        
        if (GUILayout.Button("Convert Layers"))
        {ConvertLayers();}

        if(GUILayout.Button("View Layers"))
        {
            SettingsService.OpenProjectSettings("Project/Tags and Layers");
        }
    }

    private void ConvertLayers()
    {
        if (UseSpecificParentObject)
        {

        }
        else
        {
            GameObject[] roots = FindObjectsByType<GameObject>();

            int goCount = 0;
            int ConvCount = 0;

            foreach (GameObject root in roots)
            {
                goCount++;
                if(root.layer == FromLayer)
                {
                    root.layer = ToLayer;
                    ConvCount++;
                }
            }

            Debug.Log($"Scanned {goCount} object(s), converted {ConvCount} object('s) layers.");
        }
    }
}
