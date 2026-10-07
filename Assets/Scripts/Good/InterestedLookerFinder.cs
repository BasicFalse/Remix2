using System.Collections.Generic;
using UnityEngine;
using GorillaRigging;

public class InterestedLookerFinder : MonoBehaviour
{
    public float distanceToLook;
    public Transform pointer;
    public List<RigController> rigs;
    public RigController MostInteresting;

    public void LateUpdate()
    {
        rigs = new (GorillaParent.instance.vrrigs);
        

        for (int i = 0; i < rigs.Count; i++){
            if (Vector3.Distance(rigs[i].HeadFollow.position, transform.position) > distanceToLook) {
                rigs.Remove(rigs[i]);
                i--;
            }
        }

        if(rigs.Count == 0){return;}
        
        if(!MostInteresting)
        MostInteresting = rigs[0];

        for (int i = 0; i < rigs.Count; i++)
        {
            if(rigs[i].Interest > MostInteresting.Interest)
            {
                MostInteresting = rigs[i];
            }
        }

        pointer.position = MostInteresting.HeadFollow.position;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanceToLook);
    }
#endif
}
