using UnityEngine;

public class ProxyDoor : MonoBehaviour
{
    public enum DoorType{Distance, Fixed}

    public DoorType Type;

    [Header("Door Parts")]
    public Transform topDoor;      // The top half of the door
    public Transform bottomDoor;   // The bottom half of the door

    [Header("Player Reference")]
    public Transform player;

    [Header("Door Settings")]
    public float openDistance = 3f;     // Beyond this distance, door is fully closed
    public float minDistance = 1f;      // Within this distance, door is fully open
    public float openAmount = 2f;       // How far each door moves when fully open
    public float openSpeed = 3f;        // How quickly it moves when smoothing is enabled
    public bool smooth;
    
    private bool isOpen;
    private Vector3 topClosedPos;
    private Vector3 bottomClosedPos;

    void Start()
    {
        if (topDoor) topClosedPos = topDoor.localPosition;
        if (bottomDoor) bottomClosedPos = bottomDoor.localPosition;
    }

    void Update()
    {
        //if (!player || !topDoor || !bottomDoor) return;

        
        Vector3 topTarget;
        Vector3 bottomTarget;

        if(Type == DoorType.Distance)
        {

            float dist = Vector3.Distance(player.position, transform.position);

            // open factor between minDist and openDist
            float openFactor = Mathf.InverseLerp(openDistance, minDistance, dist);
            openFactor = Mathf.Clamp01(openFactor);

            topTarget = topClosedPos + openAmount * openFactor * Vector3.up;
            bottomTarget = bottomClosedPos + openAmount * openFactor * Vector3.down;

            
        }
        else
        {
            float dist = Vector3.Distance(player.position, transform.position);

            if (dist > openDistance){isOpen = false;}
            if(dist < minDistance){isOpen = true;}

            if (isOpen)
            {
                topTarget = topClosedPos + openAmount * 1 * Vector3.up;
                bottomTarget = bottomClosedPos + openAmount * 1 * Vector3.down;
            }
            else
            {
                topTarget = topClosedPos + openAmount * 0 * Vector3.up;
                bottomTarget = bottomClosedPos + openAmount * 0 * Vector3.down;
            }
        }

        if(smooth)
        {
            if(topDoor){
            topDoor.localPosition = Vector3.Lerp(topDoor.localPosition, topTarget, Time.deltaTime * openSpeed);}

            if(bottomDoor){
            bottomDoor.localPosition = Vector3.Lerp(bottomDoor.localPosition, bottomTarget, Time.deltaTime * openSpeed);}
        }
        else
        {
            if(topDoor){
            topDoor.localPosition = topTarget;}

            if(bottomDoor){
            bottomDoor.localPosition = bottomTarget;}
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, openDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
#endif
}
