using UnityEngine;
using UnityEditor;

public class ClearLightmapData : MonoBehaviour
{
    [MenuItem("Tools/Clear Renderer Lightmap Data")]
    static void ClearAll()
    {
        foreach (var r in FindObjectsByType<MeshRenderer>())
        {
            r.lightmapIndex = -1;
            r.lightmapScaleOffset = new Vector4(1, 1, 0, 0);
            EditorUtility.SetDirty(r);
        }
        Debug.Log("Lightmap data cleared on all MeshRenderers.");
    }
}