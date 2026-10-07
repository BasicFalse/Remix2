using UnityEngine;
using UnityEngine.Android;

public class MicPerms : RemixScript
{
    public override void StartExt()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }
}
