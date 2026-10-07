using UnityEngine;

public enum FollowType {X, Y, Z, Xr, Yr, Zr, Wr}
public enum UpdateType {Frame, Late, Fixed}

public class AdvancedFollow : MonoBehaviour
{
    public FollowType[] Rules;
    public UpdateType UpdateT;

    public void Update() { if(UpdateT == UpdateType.Frame) Follow(); }

    public void LateUpdate() { if(UpdateT == UpdateType.Late) Follow(); }

    public void FixedUpdate() { if(UpdateT == UpdateType.Fixed) Follow(); }

    public void Follow()
    {
        
    }
}
