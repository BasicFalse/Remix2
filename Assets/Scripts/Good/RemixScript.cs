using System.Collections.Generic;
using UnityEngine;

public class RemixScript : MonoBehaviour
{
    private static List<RemixScript> ScriptUsers;

    private void Start()
    {
        //ScriptUsers.Add(this);
        StartExt();
    }

    public virtual void StartExt() { }

    public virtual void EventResponse() { }

    public void Broadcast()
    {
        foreach (RemixScript rs in ScriptUsers)
        {
            rs.BroadcastCast();
        }
    }
    
    public virtual void BroadcastCast() { }
}
