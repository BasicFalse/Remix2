using System.Collections.Generic;
using GorillaNetworking;
using Unity.Mathematics;
using UnityEngine;

public class MiscGrabs : RemixScript
{
    public static volatile MiscGrabs grab;

    public HitOverrideSettings HOS;
    public SpawnManager SM;
    public DataHolder Version;

    public Material MatTemp;
    public RenderTexture RendTexTemp;
    
    public OverlayBuilder Overlay;

    public List<MonsterBase> monsters;

    public List<GorillaLevelScreen> LevelScreens;

    public List<Transform> PlayerCameras;
    public List<Transform> ExtraTargs;
    

    public SkyManager SkyManager;
    public GameObject networkedVC;

    public void Awake()
    {
        if (grab == null) { grab = this; } else if (grab != this) { DestroyImmediate(this.gameObject); }
    }

    public bool DoesPlayerExist()
    {
        return GorillaComputer.instance.PlayerObjectCache != null;
    }

    public Vector3 GetPlayerPosition()
    {
        if (DoesPlayerExist())
        {
            return PlayerBase.i.transforms.Camera.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public int2 GetOverrideRange(GorillaSurfaceOverride.HitType HitNom, out int2 range)
    {
        int x;
        int y;
        switch (HitNom) {
            case GorillaSurfaceOverride.HitType.Crystal:
                x = HOS.Crystal.RangeMinimum;
                y = HOS.Crystal.RangeMaximum;
                return range = new int2(x, y);
        }
        return range = new int2(0, 0);
    }
}
