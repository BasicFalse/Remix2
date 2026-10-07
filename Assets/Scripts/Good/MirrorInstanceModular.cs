using GorillaNetworking;
using UnityEngine;

public class MirrorInstanceModular : MirrorInstance
{
    public MirrorPairModule[] MirrorPairs;
    private int i;
    public override void Start()
    {
        GorillaComputer.instance.mirrorInstances.Add(this);
        UpdateMirror(GorillaComputer.instance.MirrorQualityLevel);
        for (i = 0; i < MirrorPairs.Length; i++) MirrorPairs[i].SetupPair();
    }
    public override void UpdateMirror(int index)
    {
        for (i = 0; i < MirrorPairs.Length; i++) MirrorPairs[i].SwapMode(index);
        switch (index)
        {
            case 0:
                Material[] materials = mirrorRenderer.materials;
                for (i = 0; i < MirrorPairs.Length; i++) materials[MirrorPairs[i].MeshIndex] = MatLow;
                mirrorRenderer.materials = materials;
                break;
            case 1:
                Material[] materials1 = mirrorRenderer.materials;
                for (i = 0; i < MirrorPairs.Length; i++) materials1[MirrorPairs[i].MeshIndex] = MirrorPairs[i].RendMat;
                mirrorRenderer.materials = materials1;
                break;
            case 2:
                Material[] materials2 = mirrorRenderer.materials;
                for (i = 0; i < MirrorPairs.Length; i++) materials2[MirrorPairs[i].MeshIndex] = MatLow;
                mirrorRenderer.materials = materials2;
                break;
        }
    }
}
