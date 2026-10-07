using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureArrayToTexture2DWindow : EditorWindow
{
    private Texture2DArray textureArray;
    private string outputFolder = "Assets/ExtractedTextures";

    [MenuItem("Tools/Texture Array Extractor")]
    public static void ShowWindow()
    {
        GetWindow<TextureArrayToTexture2DWindow>("Texture Array Extractor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Texture2DArray Extractor",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        textureArray = (Texture2DArray)EditorGUILayout.ObjectField(
            "Texture Array",
            textureArray,
            typeof(Texture2DArray),
            false
        );

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        outputFolder = EditorGUILayout.TextField(
            "Output Folder",
            outputFolder
        );

        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string selectedFolder = EditorUtility.OpenFolderPanel(
                "Select Output Folder",
                Application.dataPath,
                ""
            );

            if (!string.IsNullOrEmpty(selectedFolder))
            {
                // Convert absolute Assets path to Unity project-relative path
                if (selectedFolder.StartsWith(Application.dataPath))
                {
                    outputFolder =
                        "Assets" +
                        selectedFolder.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Invalid Folder",
                        "Please select a folder inside the Unity project's Assets folder.",
                        "OK"
                    );
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(textureArray == null))
        {
            if (GUILayout.Button("Extract Textures", GUILayout.Height(35)))
            {
                ExtractTextures();
            }
        }

        EditorGUILayout.Space();

        if (textureArray != null)
        {
            EditorGUILayout.HelpBox(
                $"Texture: {textureArray.name}\n" +
                $"Size: {textureArray.width} x {textureArray.height}\n" +
                $"Slices: {textureArray.depth}",
                MessageType.Info
            );
        }
    }

    private void ExtractTextures()
    {
        if (textureArray == null)
        {
            EditorUtility.DisplayDialog(
                "Error",
                "No Texture2DArray assigned.",
                "OK"
            );
            return;
        }
    
        // Create a subfolder named after the Texture2DArray.
        string textureArrayFolder = Path.Combine(
            outputFolder,
            textureArray.name
        );
    
        if (!Directory.Exists(textureArrayFolder))
        {
            Directory.CreateDirectory(textureArrayFolder);
        }
    
        for (int i = 0; i < textureArray.depth; i++)
        {
            EditorUtility.DisplayProgressBar(
                "Extracting Texture Array",
                $"Extracting slice {i + 1} of {textureArray.depth}...",
                (float)i / textureArray.depth
            );
    
            RenderTexture rt = RenderTexture.GetTemporary(
                textureArray.width,
                textureArray.height,
                0,
                RenderTextureFormat.ARGB32
            );
    
            rt.filterMode = FilterMode.Bilinear;
    
            Material blitMaterial = new Material(
                Shader.Find("Hidden/BlitSlice")
            );
    
            if (blitMaterial == null)
            {
                RenderTexture.ReleaseTemporary(rt);
                EditorUtility.ClearProgressBar();
    
                EditorUtility.DisplayDialog(
                    "Error",
                    "Could not find shader 'Hidden/BlitSlice'.",
                    "OK"
                );
    
                return;
            }
    
            blitMaterial.SetTexture(
                "_MainTexArray",
                textureArray
            );
    
            blitMaterial.SetInt(
                "_Layer",
                i
            );
    
            Graphics.SetRenderTarget(rt);
            GL.Clear(true, true, Color.clear);
    
            Graphics.Blit(
                null,
                rt,
                blitMaterial
            );
    
            RenderTexture.active = rt;
    
            Texture2D texture = new Texture2D(
                textureArray.width,
                textureArray.height,
                TextureFormat.RGBA32,
                false
            );
    
            texture.ReadPixels(
                new Rect(
                    0,
                    0,
                    rt.width,
                    rt.height
                ),
                0,
                0
            );
    
            texture.Apply();
    
            RenderTexture.active = null;
    
            RenderTexture.ReleaseTemporary(rt);
    
            DestroyImmediate(blitMaterial);
    
            // Encode as PNG.
            byte[] bytes = texture.EncodeToPNG();
    
            // Save inside the Texture2DArray-named subfolder.
            string path = Path.Combine(
                textureArrayFolder,
                $"ExtractedTexture_{i}.png"
            );
    
            File.WriteAllBytes(path, bytes);
    
            DestroyImmediate(texture);
    
            Debug.Log($"Saved slice {i} to {path}");
        }
    
        EditorUtility.ClearProgressBar();
    
        AssetDatabase.Refresh();
    
        EditorUtility.DisplayDialog(
            "Extraction Complete",
            $"Successfully extracted {textureArray.depth} textures to:\n{textureArrayFolder}",
            "OK"
        );
    
        Debug.Log(
            $"Texture2DArray extraction complete.\n" +
            $"Extracted {textureArray.depth} textures to {textureArrayFolder}"
        );
    }


    private void OnDisable()
    {
        EditorUtility.ClearProgressBar();
    }
}
