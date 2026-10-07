using UnityEngine;
using UnityEditor;

public class LowPolyTerrainBakerWindow : EditorWindow
{
    private Terrain selectedTerrain;
    private bool createMeshCollider = true;
    private bool disableOriginalCollider = true;
    private bool flipNormals = true;
    private string saveFolder = "Assets";

    [MenuItem("Tools/Terrain/Bake Terrain Mesh")]
    public static void ShowWindow()
    {
        GetWindow<LowPolyTerrainBakerWindow>("Terrain Mesh Baker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bake Terrain Mesh", EditorStyles.boldLabel);

        selectedTerrain = (Terrain)EditorGUILayout.ObjectField("Terrain", selectedTerrain, typeof(Terrain), true);
        createMeshCollider = EditorGUILayout.Toggle("Create Mesh Collider", createMeshCollider);
        disableOriginalCollider = EditorGUILayout.Toggle("Disable Original Terrain Collider", disableOriginalCollider);
        flipNormals = EditorGUILayout.Toggle("Flip Normals", flipNormals);

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Folder", saveFolder);
        if (GUILayout.Button("Choose"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder to Save Mesh", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                saveFolder = "Assets" + path.Substring(Application.dataPath.Length);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Bake Low Poly Mesh"))
        {
            if (selectedTerrain == null)
                Debug.LogError("No terrain selected!");
            else
            {
                Mesh mesh = BakeMeshLikeUnity(selectedTerrain, flipNormals);
                if (mesh != null)
                {
                    SaveMeshAsset(mesh, selectedTerrain.name);
                    AssignMeshToTerrain(selectedTerrain, mesh, createMeshCollider, disableOriginalCollider);
                }
            }
        }
    }

    private Mesh BakeMeshLikeUnity(Terrain terrain, bool flipNormals)
    {
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;

        int lodFactor = 1 << terrain.heightmapMaximumLOD;
        int hmRes = data.heightmapResolution;
        int resolution = (hmRes - 1) / lodFactor + 1;

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        float step = 1f / (resolution - 1);

        bool[,] holes = data.GetHoles(0, 0, data.holesResolution, data.holesResolution);
        int holesRes = data.holesResolution;

        // --- Vertices ---
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float u = x * step;
                float v = z * step;
                float height = data.GetInterpolatedHeight(u, v);

                vertices[z * resolution + x] =
                    new Vector3(u * size.x, height, v * size.z);

                uvs[z * resolution + x] = new Vector2(u, v);
            }
        }

        // --- Triangles ---
        int t = 0;

        for (int z = 0; z < resolution - 1; z++)
        {
            for (int x = 0; x < resolution - 1; x++)
            {
                // Map quad center to holes grid (Unity-style)
                int holeX = Mathf.FloorToInt((float)x / (resolution - 1) * holesRes);
                int holeZ = Mathf.FloorToInt((float)z / (resolution - 1) * holesRes);

                holeX = Mathf.Clamp(holeX, 0, holesRes - 1);
                holeZ = Mathf.Clamp(holeZ, 0, holesRes - 1);

                if (!holes[holeZ, holeX])
                    continue;

                int bl = z * resolution + x;
                int br = bl + 1;
                int tl = bl + resolution;
                int tr = tl + 1;

                if (flipNormals)
                {
                    triangles[t++] = tr;
                    triangles[t++] = bl;
                    triangles[t++] = tl;

                    triangles[t++] = tr;
                    triangles[t++] = br;
                    triangles[t++] = bl;
                }
                else
                {
                    triangles[t++] = tl;
                    triangles[t++] = bl;
                    triangles[t++] = tr;

                    triangles[t++] = bl;
                    triangles[t++] = br;
                    triangles[t++] = tr;
                }
            }
        }

        System.Array.Resize(ref triangles, t);

        Mesh mesh = new Mesh();
        mesh.name = terrain.name + "_TerrainBakedMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        if (flipNormals)
        {
            Vector3[] norms = mesh.normals;
            for (int i = 0; i < norms.Length; i++)
                norms[i] = -norms[i];
            mesh.normals = norms;
        }

        mesh.RecalculateBounds();
        return mesh;
    }

    private void AssignMeshToTerrain(Terrain terrain, Mesh mesh, bool addCollider, bool disableCollider)
    {
        MeshFilter mf = terrain.gameObject.GetComponent<MeshFilter>();
        if (!mf) mf = terrain.gameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer mr = terrain.gameObject.GetComponent<MeshRenderer>();
        if (!mr) mr = terrain.gameObject.AddComponent<MeshRenderer>();
        if (!mr.sharedMaterial)
            mr.sharedMaterial = new Material(Shader.Find("Standard"));

        if (addCollider)
        {
            MeshCollider mc = terrain.gameObject.GetComponent<MeshCollider>();
            if (!mc) mc = terrain.gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;
        }

        if (disableCollider)
        {
            TerrainCollider tc = terrain.GetComponent<TerrainCollider>();
            if (tc) tc.enabled = false;
        }
    }

    private void SaveMeshAsset(Mesh mesh, string terrainName)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Mesh Asset",
            terrainName + "_TerrainBakedMesh",
            "asset",
            "Choose where to save the mesh.",
            saveFolder
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
