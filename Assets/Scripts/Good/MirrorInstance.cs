using GorillaNetworking;
using UnityEngine;

public class MirrorInstance : MonoBehaviour
{
    [System.Serializable]
    public class MirrorPairModule
    {
        public Camera Cam;
        public GameObject GoodMirror;
        public RenderTexture RendTex;
        public Material RendMat;
        public int MeshIndex;
        public void SetupPair()
        {
            RendMat = new Material(MiscGrabs.grab.MatTemp);
            RendTex = new RenderTexture(512, 512, 1, MiscGrabs.grab.RendTexTemp.format);
            RendMat.mainTexture = RendTex;
            Cam.targetTexture = RendTex;
        }
        public void SwapMode(int index)
        {
            switch (index)
            {
                case 0:
                    GoodMirror.SetActive(false);
                    Cam.enabled = false;
                    
                    break;
                case 1:
                    RendMat.mainTexture = RendTex;
                    GoodMirror.SetActive(false);
                    Cam.enabled = true;
                    break;
                case 2:
                    RendMat.mainTexture = null;
                    GoodMirror.SetActive(true);
                    Cam.enabled = false;
                    break;
            }
        }
    }
    public MeshRenderer mirrorRenderer;
    public Material MatLow;
    public Material MatMid;

    public bool MakeNewSet = false;
    
    public int RendIndex;
    public GameObject GoodMirror;
    public Camera Cam;
    private MirrorPairModule ModPair;

    public virtual void Start()
    {
        GorillaComputer.instance.mirrorInstances.Add(this);
        if(MakeNewSet)
        {
            ModPair = new MirrorPairModule
            { Cam = Cam, GoodMirror = GoodMirror, MeshIndex = RendIndex };
            ModPair.SetupPair();
            MatMid = ModPair.RendMat;
        }
        UpdateMirror(GorillaComputer.instance.MirrorQualityLevel);
    }

    public virtual void UpdateMirror(int index)
    {
        if(ModPair == null){
        switch (index)
        {
            case 0:
                Material[] materials = mirrorRenderer.materials;
                materials[RendIndex] = MatLow;
                mirrorRenderer.materials = materials;
                GoodMirror.SetActive(false);
                Cam.enabled = false;
                break;
            case 1:
                Material[] materials1 = mirrorRenderer.materials;
                materials1[RendIndex] = MatMid;
                mirrorRenderer.materials = materials1;
                GoodMirror.SetActive(false);
                Cam.enabled = true;
                break;
            case 2:
                Material[] materials2 = mirrorRenderer.materials;
                materials2[RendIndex] = MatLow;
                mirrorRenderer.materials = materials2;
                GoodMirror.SetActive(true);
                Cam.enabled = false;
                break;
        }
        } else {
            ModPair.SwapMode(index);
            if(index == 0)
            {
                Material[] materials = mirrorRenderer.materials;
                materials[ModPair.MeshIndex] = MatLow;
                mirrorRenderer.materials = materials;
            }
            else if(index == 1)
            {
                Material[] materials1 = mirrorRenderer.materials;
                materials1[ModPair.MeshIndex] = MatMid;
                mirrorRenderer.materials = materials1;
            }
            else if(index == 2)
            {
                Material[] materials2 = mirrorRenderer.materials;
                materials2[ModPair.MeshIndex] = MatLow;
                mirrorRenderer.materials = materials2;
            }
        }
    }
}
