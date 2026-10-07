using UnityEditor;
using UnityEngine;
using System.IO;

public class TextureEditor : EditorWindow
{
    // -------- ENUMS --------
    public enum BlurType
    {
        None,
        Box,
        Gaussian,
        Directional
    }

    // -------- TEXTURE --------
    private Texture2D sourceTexture;

    // -------- COLOR --------
    private float hueShift = 0f;
    private float saturation = 1f;
    private float contrast = 1f;
    private float brightness = 0f;
    private float gamma = 1f;
    private bool invertColors = false;

    // -------- BLUR --------
    private BlurType blurType = BlurType.None;
    private float blurStrength = 0f;
    private float blurAngle = 0f;

    // -------- OPTIONS --------
    private bool replaceSourceTexture = false;
    private bool saveToSameFolder = true;
    private bool livePreview = true;

    // -------- INTERNAL --------
    private bool wasDraggingSlider = false;
    private Material previewMaterial;

    private const string ShaderName = "Hidden/Editor/TextureColorAdjustPreview";

    [MenuItem("Tools/Texture Color Adjust")]
    public static void Open()
    {
        GetWindow<TextureEditor>("Texture Editor");
    }

    private void OnEnable()
    {
        Shader shader = Shader.Find(ShaderName);
        if (shader != null)
        {
            previewMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }

    private void OnDisable()
    {
        if (previewMaterial != null)
            DestroyImmediate(previewMaterial);
    }

    private void OnGUI()
    {
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Source Texture",
            sourceTexture,
            typeof(Texture2D),
            false
        );

        GUILayout.Space(6);
        GUILayout.Label("Color Adjustments", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        hueShift = EditorGUILayout.Slider("Hue Shift", hueShift, 0f, 360f);
        saturation = EditorGUILayout.Slider("Saturation", saturation, 0f, 2f);
        contrast = EditorGUILayout.Slider("Contrast", contrast, 0f, 2f);
        brightness = EditorGUILayout.Slider("Brightness", brightness, -1f, 1f);
        gamma = EditorGUILayout.Slider("Gamma", gamma, 0.1f, 3f);
        invertColors = EditorGUILayout.Toggle("Invert Colors", invertColors);

        GUILayout.Space(6);
        GUILayout.Label("Blur", EditorStyles.boldLabel);

        blurType = (BlurType)EditorGUILayout.EnumPopup("Blur Type", blurType);
        blurStrength = EditorGUILayout.Slider("Blur Strength", blurStrength, 0f, 5f);

        if (blurType == BlurType.Directional)
            blurAngle = EditorGUILayout.Slider("Blur Angle", blurAngle, 0f, 360f);

        bool valuesChanged = EditorGUI.EndChangeCheck();

        GUILayout.Space(6);
        livePreview = EditorGUILayout.ToggleLeft("Live Preview While Dragging", livePreview);

        GUILayout.Space(6);
        replaceSourceTexture = EditorGUILayout.ToggleLeft(
            "Replace Source Texture (Overwrite)",
            replaceSourceTexture
        );

        using (new EditorGUI.DisabledScope(replaceSourceTexture))
        {
            saveToSameFolder = EditorGUILayout.ToggleLeft(
                "Save To Same Folder As Source",
                saveToSameFolder
            );
        }

        HandlePreviewUpdate(valuesChanged);

        GUILayout.Space(10);
        DrawSplitPreview();
        GUILayout.Space(10);

        GUI.enabled = sourceTexture != null;
        if (GUILayout.Button("Apply", GUILayout.Height(32)))
        {
            ApplyTextureCPU();
        }
        GUI.enabled = true;
    }

    // -------- PREVIEW --------
    private void HandlePreviewUpdate(bool valuesChanged)
    {
        bool isDragging = EditorGUIUtility.hotControl != 0;

        if (!valuesChanged) return;

        if (livePreview)
            Repaint();
        else if (!isDragging && wasDraggingSlider)
        {
            wasDraggingSlider = false;
            Repaint();
        }
        else if (isDragging)
            wasDraggingSlider = true;
    }

    private void DrawSplitPreview()
    {
        GUILayout.Label("Preview (Before / After)", EditorStyles.boldLabel);

        if (sourceTexture == null)
        {
            GUILayout.Box("No Preview", GUILayout.Width(256), GUILayout.Height(128));
            return;
        }

        Rect rect = GUILayoutUtility.GetRect(256, 128);
        Rect left = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
        Rect right = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);

        EditorGUI.DrawPreviewTexture(left, sourceTexture);

        if (previewMaterial != null)
        {
            previewMaterial.SetFloat("_Hue", hueShift);
            previewMaterial.SetFloat("_Saturation", saturation);
            previewMaterial.SetFloat("_Contrast", contrast);
            previewMaterial.SetFloat("_Brightness", brightness);
            previewMaterial.SetFloat("_Gamma", gamma);
            previewMaterial.SetFloat("_Invert", invertColors ? 1f : 0f);
            previewMaterial.SetFloat("_BlurType", (float)blurType);
            previewMaterial.SetFloat("_BlurStrength", blurStrength);
            previewMaterial.SetFloat("_BlurAngle", blurAngle);

            Graphics.DrawTexture(right, sourceTexture, previewMaterial);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(right, sourceTexture);
        }

        EditorGUI.DrawRect(
            new Rect(rect.x + rect.width / 2f - 1, rect.y, 2, rect.height),
            Color.black
        );
    }

    // -------- APPLY --------
    private void ApplyTextureCPU()
    {
        Texture2D baked = CreateAdjustedTextureCPU(sourceTexture);

        string path;
        if (replaceSourceTexture)
            path = AssetDatabase.GetAssetPath(sourceTexture);
        else if (saveToSameFolder)
        {
            string src = AssetDatabase.GetAssetPath(sourceTexture);
            path = Path.Combine(Path.GetDirectoryName(src), sourceTexture.name + "_Adjusted.png");
        }
        else
        {
            path = EditorUtility.SaveFilePanelInProject(
                "Save Adjusted Texture",
                sourceTexture.name + "_Adjusted",
                "png",
                "Choose save location"
            );
        }

        if (string.IsNullOrEmpty(path)) return;

        File.WriteAllBytes(path, baked.EncodeToPNG());
        AssetDatabase.Refresh();
    }

    // -------- CPU PROCESS --------
    private Texture2D CreateAdjustedTextureCPU(Texture2D source)
    {
        Texture2D readable = GetReadableTexture(source);
        int w = readable.width;
        int h = readable.height;

        Color[] pixels = readable.GetPixels();
        Color[] original = (Color[])pixels.Clone();

        int radius = Mathf.RoundToInt(blurStrength);

        if (blurType != BlurType.None && radius > 0)
        {
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                pixels[x + y * w] = blurType switch
                {
                    BlurType.Box => SampleBoxBlur(original, x, y, w, h, radius),
                    BlurType.Gaussian => SampleGaussianBlur(original, x, y, w, h),
                    BlurType.Directional => SampleDirectionalBlur(original, x, y, w, h, radius, blurAngle),
                    _ => original[x + y * w]
                };
            }
        }

        float hueOffset = hueShift / 360f;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];

            Color.RGBToHSV(c, out float h1, out float s1, out float v1);
            h1 = (h1 + hueOffset) % 1f;
            s1 = Mathf.Clamp01(s1 * saturation);
            c = Color.HSVToRGB(h1, s1, v1);
            c.a = pixels[i].a;

            Vector3 rgb = ((new Vector3(c.r, c.g, c.b) - Vector3.one * 0.5f) * contrast) + Vector3.one * 0.5f;
            rgb += Vector3.one * brightness;
            rgb = new Vector3(
                Mathf.Pow(Mathf.Max(0f, rgb.x), 1f / gamma),
                Mathf.Pow(Mathf.Max(0f, rgb.y), 1f / gamma),
                Mathf.Pow(Mathf.Max(0f, rgb.z), 1f / gamma)
            );

            c = new Color(
                Mathf.Clamp01(rgb.x),
                Mathf.Clamp01(rgb.y),
                Mathf.Clamp01(rgb.z),
                c.a
            );

            if (invertColors)
            {
                c.r = 1f - c.r;
                c.g = 1f - c.g;
                c.b = 1f - c.b;
            }

            pixels[i] = c;
        }

        Texture2D result = new Texture2D(w, h, TextureFormat.RGBA32, false);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    // -------- BLUR HELPERS --------
    private Color SampleBoxBlur(Color[] p, int x, int y, int w, int h, int r)
    {
        Color sum = Color.black;
        int count = 0;

        for (int oy = -r; oy <= r; oy++)
        for (int ox = -r; ox <= r; ox++)
        {
            int nx = Mathf.Clamp(x + ox, 0, w - 1);
            int ny = Mathf.Clamp(y + oy, 0, h - 1);
            sum += p[nx + ny * w];
            count++;
        }

        return sum / count;
    }

    private Color SampleGaussianBlur(Color[] p, int x, int y, int w, int h)
    {
        return p[x + y * w] * 0.4f + SampleBoxBlur(p, x, y, w, h, 1) * 0.6f;
    }

    private Color SampleDirectionalBlur(Color[] p, int x, int y, int w, int h, int r, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        int dx = Mathf.RoundToInt(Mathf.Cos(rad) * r);
        int dy = Mathf.RoundToInt(Mathf.Sin(rad) * r);

        Color sum = Color.black;
        int count = 0;

        for (int i = -r; i <= r; i++)
        {
            int nx = Mathf.Clamp(x + dx * i, 0, w - 1);
            int ny = Mathf.Clamp(y + dy * i, 0, h - 1);
            sum += p[nx + ny * w];
            count++;
        }

        return sum / count;
    }

    private Texture2D GetReadableTexture(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear
        );

        Graphics.Blit(source, rt);
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }
}
