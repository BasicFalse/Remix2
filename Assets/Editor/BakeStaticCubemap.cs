using UnityEngine;
using UnityEditor;
using System.IO;

public class BakeStaticCubemap : ScriptableWizard
{

    public Transform renderPosition;
    public Cubemap cubemap;
    // Camera settings.
    public int cameraDepth = 24;
    public LayerMask cameraLayerMask = -1;
    public Color cameraBackgroundColor;
    public float cameraNearPlane = 0.1f;
    public float cameraFarPlane = 2500.0f;
    public int imageSize;
    public string spefDirectory;
    public bool cameraUseOcclusion = true;
    // Cubemap settings.
    public FilterMode cubemapFilterMode = FilterMode.Trilinear;
    // Quality settings.
    public int antiAliasing = 4;

    public bool createIndividualImages = false;

    // The folder where individual cubemap images will be saved
    static readonly string imageDirectory = "Assets/Resources/Skys/Images";
    static readonly string[] cubemapImage
         = new string[]{"front+Z", "right+X", "back-Z", "left-X", "top+Y", "bottom-Y"};
           static readonly Vector3[] eulerAngles = new Vector3[]{new(0.0f,0.0f,0.0f),
           new(0.0f,-90.0f,0.0f), new(0.0f,180.0f,0.0f), 
           new(0.0f,90.0f,0.0f), new(-90.0f,0.0f,0.0f), 
           new(90.0f,0.0f,0.0f)};
    
    void OnWizardUpdate()
    {
        helpString = "Set the position to render from and the cubemap to bake.";
        if(renderPosition != null && cubemap != null && imageSize != 0)
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
    }
    
    void OnWizardCreate ()
    {

        // Create temporary camera for rendering.
        GameObject go = new GameObject( "CubemapCam", typeof(Camera) );
        // Camera settings. 
        go.GetComponent<Camera>().depth = cameraDepth;
        go.GetComponent<Camera>().backgroundColor = cameraBackgroundColor;
        go.GetComponent<Camera>().cullingMask = cameraLayerMask;
        go.GetComponent<Camera>().nearClipPlane = cameraNearPlane;
        go.GetComponent<Camera>().farClipPlane = cameraFarPlane;
        go.GetComponent<Camera>().useOcclusionCulling = cameraUseOcclusion;
        // Cubemap settings
        cubemap.filterMode = cubemapFilterMode;
        // Set antialiasing
        QualitySettings.antiAliasing = antiAliasing;

        // Place the camera on the render position.
        go.transform.position = renderPosition.position;
        go.transform.rotation = Quaternion.identity;

        // Bake the cubemap
        go.GetComponent<Camera>().RenderToCubemap(cubemap);

        // Rendering individual images
        if(createIndividualImages)
        {
            if (spefDirectory == null)
            {
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }
            }
            else
            {
                if (!Directory.Exists(new string(imageDirectory + "/" + spefDirectory)))
                {
                    Directory.CreateDirectory(new string(imageDirectory + "/" + spefDirectory));
                }
            }
            
            RenderIndividualCubemapImages(go);
        }

        
        // Destroy the camera after rendering.
        DestroyImmediate(go);
    }

    void RenderIndividualCubemapImages(GameObject go)
    {
        go.GetComponent<Camera>().backgroundColor = Color.black;
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        go.GetComponent<Camera>().fieldOfView = 90.0f;    
        go.GetComponent<Camera>().aspect = 1.0f;
        
        go.transform.rotation = Quaternion.identity;

        //Render individual images        
        for (int camOrientation = 0; camOrientation < eulerAngles.Length ; camOrientation++)
        {
            string imgPath = imageDirectory;
            if (spefDirectory != null)
            { imgPath = imageDirectory + "/" + spefDirectory; }

                string imageName = Path.Combine(imgPath, cubemap.name + "_"
                        + cubemapImage[camOrientation] + ".png");
                go.GetComponent<Camera>().transform.eulerAngles = eulerAngles[camOrientation];
                RenderTexture renderTex = new RenderTexture(imageSize,
                        imageSize, cameraDepth);
                go.GetComponent<Camera>().targetTexture = renderTex;
                go.GetComponent<Camera>().Render();
                RenderTexture.active = renderTex;

                Texture2D img = new Texture2D(imageSize, imageSize,
                        TextureFormat.RGB24, false);
                img.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);

                RenderTexture.active = null;
                GameObject.DestroyImmediate(renderTex);

                byte[] imgBytes = img.EncodeToPNG();
                File.WriteAllBytes(imageName, imgBytes);

                AssetDatabase.ImportAsset(imageName, ImportAssetOptions.ForceUpdate);
            }

        AssetDatabase.Refresh();
    }

    [MenuItem("GameObject/Bake Cubemap")]
    static void RenderCubemap ()
    {
        ScriptableWizard.DisplayWizard("Bake CubeMap", typeof(BakeStaticCubemap),"Bake!");
    }
}
