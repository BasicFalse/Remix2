using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BarycentricMesh : MonoBehaviour
{
    void Start()
    {
        var mf = GetComponent<MeshFilter>();
        var original = mf.sharedMesh;
        var newMesh = new Mesh();

        var verts = original.vertices;
        var tris = original.triangles;

        Vector3[] newVerts = new Vector3[tris.Length];
        Vector3[] barys = new Vector3[tris.Length];
        int[] newTris = new int[tris.Length];

        for (int i = 0; i < tris.Length; i += 3)
        {
            newVerts[i + 0] = verts[tris[i + 0]];
            newVerts[i + 1] = verts[tris[i + 1]];
            newVerts[i + 2] = verts[tris[i + 2]];

            barys[i + 0] = new Vector3(1, 0, 0);
            barys[i + 1] = new Vector3(0, 1, 0);
            barys[i + 2] = new Vector3(0, 0, 1);

            newTris[i + 0] = i + 0;
            newTris[i + 1] = i + 1;
            newTris[i + 2] = i + 2;
        }

        newMesh.vertices = newVerts;
        newMesh.triangles = newTris;
        newMesh.SetUVs(0, barys); // stored in TEXCOORD0

        newMesh.RecalculateNormals();
        mf.mesh = newMesh;
    }
}
