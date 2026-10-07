using UnityEngine;
using UnityEditor;
using System.IO;

public class SkinnedMeshBakerWindow : EditorWindow
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private string outputFileName = "BakedMesh";
    private string outputFolder = "Assets/Mesh";
    private bool createPrefab = false;
    private bool pingAfterExport = true;

    [MenuItem("Tools/Skinned Mesh Baker")]
    public static void ShowWindow()
    {
        var window = GetWindow<SkinnedMeshBakerWindow>("Skinned Mesh Baker");
        window.minSize = new Vector2(320, 260);
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        // Header
        var headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("Skinned Mesh Baker", headerStyle);
        GUILayout.Space(4);
        EditorGUILayout.HelpBox(
            "Bakes the current pose of a SkinnedMeshRenderer into a static Mesh asset.",
            MessageType.None);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
            "Skinned Mesh Renderer",
            skinnedMeshRenderer,
            typeof(SkinnedMeshRenderer),
            true);

        // Auto-fill filename from the renderer's name
        if (EditorGUI.EndChangeCheck() && skinnedMeshRenderer != null)
        {
            outputFileName = skinnedMeshRenderer.name + "_Baked";
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

        outputFolder = EditorGUILayout.TextField("Save Folder", outputFolder);

        using (new EditorGUILayout.HorizontalScope())
        {
            outputFileName = EditorGUILayout.TextField("File Name", outputFileName);
            GUILayout.Label(".asset", GUILayout.Width(40));
        }

        GUILayout.Space(6);
        createPrefab = EditorGUILayout.Toggle("Also Create Prefab", createPrefab);
        pingAfterExport = EditorGUILayout.Toggle("Ping in Project", pingAfterExport);

        GUILayout.Space(14);

        // Validation feedback
        bool canExport = skinnedMeshRenderer != null && !string.IsNullOrWhiteSpace(outputFileName);
        if (skinnedMeshRenderer == null)
            EditorGUILayout.HelpBox("Assign a SkinnedMeshRenderer to continue.", MessageType.Warning);

        using (new EditorGUI.DisabledScope(!canExport))
        {
            if (GUILayout.Button("Bake & Export Mesh", GUILayout.Height(36)))
                BakeAndExport();
        }

        GUILayout.Space(6);
    }

    private void BakeAndExport()
    {
        // Bake the current pose into a new Mesh
        var bakedMesh = new Mesh
        {
            name = outputFileName
        };
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Ensure output folder exists
        if (!AssetDatabase.IsValidFolder(outputFolder))
            CreateFolderRecursive(outputFolder);

        // Build asset path and save
        string assetPath = $"{outputFolder}/{outputFileName}.asset";
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(bakedMesh, assetPath);

        // Optionally create a prefab with a MeshFilter + MeshRenderer
        if (createPrefab)
        {
            string prefabPath = Path.ChangeExtension(assetPath, ".prefab");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

            var go = new GameObject(outputFileName);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();

            mf.sharedMesh = bakedMesh;

            // Copy materials from the skinned renderer
            mr.sharedMaterials = skinnedMeshRenderer.sharedMaterials;

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);

            Debug.Log($"[SkinnedMeshBaker] Prefab saved to: {prefabPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SkinnedMeshBaker] Mesh baked and saved to: {assetPath}");

        if (pingAfterExport)
        {
            var savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            EditorGUIUtility.PingObject(savedMesh);
            Selection.activeObject = savedMesh;
        }
    }

    /// <summary>
    /// Recursively creates any missing folders in a path like "Assets/Foo/Bar/Baz".
    /// </summary>
    private static void CreateFolderRecursive(string path)
    {
        // Split on forward slash; Unity paths always use /
        string[] parts = path.Split('/');
        string current = parts[0]; // should be "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}