using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ProBuilderVertCompresser : EditorWindow
{
    private float mergeDistance = 0.05f;
    private int maxGroupSize = 4;
    private bool createNewMesh = false;
    private string newMeshSuffix = "_compressed";

    private ProBuilderMesh targetMesh;
    private List<List<int>> previewGroups = new List<List<int>>();
    private int lastMergeCount = 0;

    [MenuItem("Tools/Vertex Compresser 9000")]
    public static void ShowWindow()
    {
        GetWindow<ProBuilderVertCompresser>("Vertex Compresser 9000");
    }

    private void OnGUI()
    {
        GUILayout.Label("Vertex Compresser 9000", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // Auto-grab from selection
        ProBuilderMesh selected = Selection.activeGameObject?.GetComponent<ProBuilderMesh>();
        if (selected != null && selected != targetMesh)
        {
            targetMesh = selected;
            previewGroups.Clear();
        }

        targetMesh = (ProBuilderMesh)EditorGUILayout.ObjectField("Target Mesh", targetMesh, typeof(ProBuilderMesh), true);

        EditorGUILayout.Space(6);
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        mergeDistance = EditorGUILayout.Slider("Merge Distance", mergeDistance, 0.001f, 1.0f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Max Group Size", GUILayout.Width(120));
        if (GUILayout.Toggle(maxGroupSize == 2, "2", "Button", GUILayout.Width(36))) maxGroupSize = 2;
        if (GUILayout.Toggle(maxGroupSize == 3, "3", "Button", GUILayout.Width(36))) maxGroupSize = 3;
        if (GUILayout.Toggle(maxGroupSize == 4, "4", "Button", GUILayout.Width(36))) maxGroupSize = 4;
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        // --- New Mesh Option ---
        GUILayout.Label("Output", EditorStyles.boldLabel);
        createNewMesh = EditorGUILayout.Toggle("Create New Mesh", createNewMesh);

        if (createNewMesh)
        {
            EditorGUI.indentLevel++;
            newMeshSuffix = EditorGUILayout.TextField("Name Suffix", newMeshSuffix);
            EditorGUI.indentLevel--;
            EditorGUILayout.HelpBox(
                "A duplicate of the target GameObject will be created with the suffix appended. " +
                "The original will not be modified.",
                MessageType.None);
        }

        EditorGUILayout.Space(6);

        // Preview
        GUI.backgroundColor = new Color(0.6f, 0.8f, 1f);
        if (GUILayout.Button("Scan for Groups (Preview)", GUILayout.Height(28)))
        {
            if (targetMesh != null)
                ScanMesh(dryRun: true);
            else
                Debug.LogWarning("No ProBuilderMesh selected.");
        }
        GUI.backgroundColor = Color.white;

        if (previewGroups.Count > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                $"Found {previewGroups.Count} group(s) to merge " +
                $"({previewGroups.Sum(g => g.Count)} verts → {previewGroups.Count} verts, " +
                $"saving {previewGroups.Sum(g => g.Count) - previewGroups.Count} verts)",
                MessageType.Info);
        }

        EditorGUILayout.Space(4);

        // Apply
        string applyLabel = createNewMesh ? "Collapse Vertices (New Mesh)" : "Collapse Vertices";
        GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
        EditorGUI.BeginDisabledGroup(targetMesh == null);
        if (GUILayout.Button(applyLabel, GUILayout.Height(32)))
        {
            if (targetMesh != null)
            {
                if (createNewMesh)
                    ApplyToNewMesh();
                else
                {
                    Undo.RegisterCompleteObjectUndo(targetMesh, "ProBuilder Collapse Verts");
                    ScanMesh(dryRun: false);
                }
            }
        }
        EditorGUI.EndDisabledGroup();
        GUI.backgroundColor = Color.white;

        if (lastMergeCount > 0)
        {
            EditorGUILayout.Space(4);
            string resultLabel = createNewMesh
                ? $"Last run: merged {lastMergeCount} group(s) on new mesh '{targetMesh?.name}{newMeshSuffix}'."
                : $"Last run: merged {lastMergeCount} group(s).";
            EditorGUILayout.HelpBox(resultLabel, MessageType.None);
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Groups of 2-4 verts within Merge Distance will be collapsed to their average position.\n" +
            "A vert won't appear in more than one group.",
            MessageType.None);
    }

    /// <summary>
    /// Duplicates the target GameObject, applies the collapse to the duplicate,
    /// and selects it in the hierarchy.
    /// </summary>
    private void ApplyToNewMesh()
    {
        // Duplicate the full GameObject (preserves transform, components, children)
        GameObject duplicate = Instantiate(targetMesh.gameObject, targetMesh.transform.parent);
        duplicate.name = targetMesh.name + newMeshSuffix;

        // Position it identically (Instantiate already copies transform, but be explicit)
        duplicate.transform.SetPositionAndRotation(
            targetMesh.transform.position,
            targetMesh.transform.rotation);
        duplicate.transform.localScale = targetMesh.transform.localScale;

        // Register the new object for Undo so it can be removed cleanly
        Undo.RegisterCreatedObjectUndo(duplicate, "ProBuilder Collapse Verts (New Mesh)");

        ProBuilderMesh dupMesh = duplicate.GetComponent<ProBuilderMesh>();
        if (dupMesh == null)
        {
            Debug.LogError("[VertCollapser] Duplicate has no ProBuilderMesh component.");
            DestroyImmediate(duplicate);
            return;
        }

        // Run the collapse on the duplicate
        ProBuilderMesh savedTarget = targetMesh;
        targetMesh = dupMesh;
        ScanMesh(dryRun: false);
        targetMesh = savedTarget;

        // Select the new mesh in the hierarchy
        Selection.activeGameObject = duplicate;

        Debug.Log($"[VertCollapser] Created new mesh '{duplicate.name}' with {lastMergeCount} group(s) merged.");
    }

    private void ScanMesh(bool dryRun)
    {
        previewGroups.Clear();
        lastMergeCount = 0;

        IList<Vector3> positions = targetMesh.positions;
        int count = positions.Count;

        bool[] claimed = new bool[count];
        List<List<int>> groups = new List<List<int>>();

        for (int i = 0; i < count; i++)
        {
            if (claimed[i]) continue;

            List<int> group = new List<int> { i };

            for (int j = i + 1; j < count && group.Count < maxGroupSize; j++)
            {
                if (claimed[j]) continue;

                bool closeToAll = group.All(g =>
                    Vector3.Distance(positions[g], positions[j]) <= mergeDistance);

                if (closeToAll)
                    group.Add(j);
            }

            if (group.Count >= 2)
            {
                foreach (int idx in group)
                    claimed[idx] = true;

                groups.Add(group);
            }
        }

        previewGroups = groups;

        if (dryRun)
        {
            Repaint();
            return;
        }

        foreach (List<int> group in groups)
        {
            targetMesh.MergeVertices(group.ToArray(), false);
        }

        lastMergeCount = groups.Count;

        targetMesh.ToMesh();
        targetMesh.Refresh();

        EditorUtility.SetDirty(targetMesh);

        Debug.Log($"[VertCollapser] Merged {groups.Count} group(s) on '{targetMesh.name}'.");
        previewGroups.Clear();
        Repaint();
    }
}