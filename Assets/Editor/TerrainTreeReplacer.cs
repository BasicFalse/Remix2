using UnityEngine;
using UnityEditor;

public class TerrainTreeReplacer : EditorWindow
{
    private Terrain terrain;
    private GameObject replacementPrefab;
    private bool removeTerrainTrees = true;
    private bool randomRotation = false;
    private bool replaceScale = false;
    private bool addToScale = false;
    private float scaleOverride = 0f;

    [MenuItem("Tools/Terrain/Replace Trees")]
    public static void ShowWindow()
    {
        GetWindow<TerrainTreeReplacer>("Terrain Tree Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Tree Replacer", EditorStyles.boldLabel);

        terrain = (Terrain)EditorGUILayout.ObjectField(
            "Terrain",
            terrain,
            typeof(Terrain),
            true
        );

        replacementPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Replacement Prefab",
            replacementPrefab,
            typeof(GameObject),
            false
        );

        removeTerrainTrees = EditorGUILayout.Toggle(
            "Remove Terrain Trees",
            removeTerrainTrees
        );

        randomRotation = EditorGUILayout.Toggle(
            "Random Rotation",
            randomRotation
        );

        replaceScale = EditorGUILayout.Toggle(
            "Replace Scale",
            replaceScale
        );

        addToScale = EditorGUILayout.Toggle(
            "Add To Scale",
            addToScale
        );

        scaleOverride = EditorGUILayout.FloatField(
            "Scale Override",
            scaleOverride
        );



        GUILayout.Space(10);

        GUI.enabled = terrain != null && replacementPrefab != null;

        if (GUILayout.Button("Replace Trees"))
        {
            ReplaceTrees();
        }

        GUI.enabled = true;
    }

    private void ReplaceTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        TreeInstance[] trees = terrainData.treeInstances;

        Undo.RegisterFullObjectHierarchyUndo(terrain.gameObject, "Replace Terrain Trees");

        Transform parent = new GameObject("Replaced Trees").transform;
        parent.position = terrain.transform.position;

        foreach (TreeInstance tree in trees)
        {
            Vector3 worldPos = Vector3.Scale(tree.position, terrainData.size)
                               + terrain.transform.position;

            Quaternion rotation = Quaternion.Euler(0f, tree.rotation * Mathf.Rad2Deg, 0f);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);
            instance.transform.position = worldPos;
            instance.transform.rotation = rotation;
            instance.transform.localScale = Vector3.one * tree.widthScale;
            instance.transform.SetParent(parent);

            if(randomRotation)
            {
                float randomY = Random.Range(0f, 360f);
                instance.transform.rotation = Quaternion.Euler(0f, randomY, 0f);
            }

            if (replaceScale)
            {
                if (addToScale)
                {
                    Vector3 OS = instance.transform.localScale;
                    OS.x += scaleOverride;
                    OS.y += scaleOverride;
                    OS.z += scaleOverride;

                    instance.transform.localScale = OS;
                }
                else
                {
                    instance.transform.localScale = new Vector3(scaleOverride, scaleOverride, scaleOverride);
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, "Create Tree Prefab");
        }

        if (removeTerrainTrees)
        {
            terrainData.treeInstances = new TreeInstance[0];
            EditorUtility.SetDirty(terrainData);
        }

        Debug.Log($"Replaced {trees.Length} terrain trees with prefab instances.");
    }
}
