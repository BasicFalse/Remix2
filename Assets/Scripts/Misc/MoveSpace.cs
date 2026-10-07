using UnityEngine;

public class MoveSpace : MonoBehaviour
{
    public Vector3 lastPosition;
    public Vector3 currPosition;

    public bool active;
    public int priority;

    public void Update()
    {
        lastPosition = currPosition;
        currPosition = transform.position;
    }
}
