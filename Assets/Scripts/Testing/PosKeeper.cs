using Unity.Mathematics;
using UnityEngine;

public class PosKeeper : MonoBehaviour
{
    public Vector3 pos;
    public quaternion rot;
    public bool setPos;
    public bool local;
    public Vector3 OveridePosition;

    void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
    }
    void Update()
    {
        if (setPos)
        {
            if (local)
            {
                this.transform.SetLocalPositionAndRotation(OveridePosition, rot);
            }
            else {this.transform.SetPositionAndRotation(OveridePosition, rot); }
        }
        else
        {
            this.transform.SetPositionAndRotation(pos, rot);
        }
    }
}
