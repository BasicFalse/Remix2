using System.Collections.Generic;
using UnityEngine;

public class MoveSpaceDetector : MonoBehaviour
{
    public bool InMoveSpace;

    public List<MoveSpace> colliders;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MoveSpace>()){
            colliders.Add(other.GetComponent<MoveSpace>());
            InMoveSpace = colliders.Count > 0;}
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<MoveSpace>()){
            colliders.Remove(other.GetComponent<MoveSpace>());
            InMoveSpace = colliders.Count > 0;}
    }

    public MoveSpace GetHighestPriority()
    {
        if(colliders.Count != 0){
            int highest = 0;
            for (int i = 0; i<colliders.Count; i++)
            {
                if(colliders[i].priority > colliders[highest].priority && colliders[i].active)
                {
                    highest = i;
                }
            }
            return colliders[highest];
        }

        return null;
    }
}
