using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsolateToSceneTool : EditorWindow
{
    private GameObject targetObject;
    private string newSceneName = "IsolatedScene";
    private string saveFolder = "Assets/Scenes";
    private bool openAfterCreation = true;
    private bool additive = false;

    [MenuItem("Tools/Isolate Object to Scene")]
    public static void OpenWindow()
    {
        var window = GetWindow<IsolateToSceneTool>("Isolate to Scene");
        window.minSize = new Vector2(320, 260);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Isolate GameObject to Its Own Scene", EditorStyles.boldLabel);
        GUILayout.Space(6);

        EditorGUILayout.HelpBox(
            "This tool moves a selected GameObject into its own scene file, preserving its world position and rotation.",
            MessageType.Info
        );

        GUILayout.Space(10);

        // Target object field
        targetObject = (GameObject)EditorGUILayout.ObjectField(
            "Target Object",
            targetObject,
            typeof(GameObject),
            true
        );

        // Auto-fill name from object
        if (targetObject != null && newSceneName == "IsolatedScene")
            newSceneName = targetObject.name + "_Scene";

        GUILayout.Space(6);

        newSceneName = EditorGUILayout.TextField("New Scene Name", newSceneName);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        GUILayout.Space(6);

        openAfterCreation = EditorGUILayout.Toggle("Open Scene After Creation", openAfterCreation);
        additive = EditorGUILayout.Toggle("Load Additively (keep current scene)", additive);

        GUILayout.Space(14);

        GUI.enabled = targetObject != null && !string.IsNullOrWhiteSpace(newSceneName);

        if (GUILayout.Button("Isolate to New Scene", GUILayout.Height(36)))
            IsolateObject();

        GUI.enabled = true;

        GUILayout.Space(6);

        if (targetObject == null)
            EditorGUILayout.HelpBox("Assign a GameObject to continue.", MessageType.Warning);
    }

    private void IsolateObject()
    {
        // Store transform data before any scene operations
        Vector3 worldPos = targetObject.transform.position;
        Quaternion worldRot = targetObject.transform.rotation;
        Vector3 worldScale = targetObject.transform.lossyScale;

        string scenePath = $"{saveFolder}/{newSceneName}.unity";

        // Ensure save folder exists
        if (!AssetDatabase.IsValidFolder(saveFolder))
        {
            CreateFolderRecursive(saveFolder);
        }

        // Create the new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        newScene.name = newSceneName;

        // Move the object into the new scene
        SceneManager.MoveGameObjectToScene(targetObject, newScene);

        // Restore world transform (moving scenes can alter parenting)
        targetObject.transform.position = worldPos;
        targetObject.transform.rotation = worldRot;

        // Restore lossy scale by applying to localScale
        // (only works correctly if no parent; with a parent, lossyScale restoring is complex)
        if (targetObject.transform.parent == null)
            targetObject.transform.localScale = worldScale;

        // Save the new scene
        bool saved = EditorSceneManager.SaveScene(newScene, scenePath);

        if (saved)
        {
            Debug.Log($"[IsolateToScene] Scene saved to: {scenePath}");

            if (openAfterCreation)
            {
                if (additive)
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
                else
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Success",
                $"'{targetObject.name}' has been isolated to:\n{scenePath}",
                "OK"
            );

            // Reset fields
            targetObject = null;
            newSceneName = "IsolatedScene";
        }
        else
        {
            Debug.LogError("[IsolateToScene] Failed to save the new scene.");
            EditorUtility.DisplayDialog("Error", "Scene could not be saved. Check the save folder path.", "OK");
        }
    }

    private void CreateFolderRecursive(string path)
    {
        string[] parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}