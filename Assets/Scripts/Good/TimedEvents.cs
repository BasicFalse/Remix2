using UnityEngine;

public class TimedEvents : MonoBehaviour
{
    public Material EyeSky;
    public void CheckTimedEvents(int hourLocal)
    {
        if(hourLocal == 3)
        {
            MiscGrabs.grab.SkyManager.SetOverrideSky(true, EyeSky);
            //RenderSettings.skybox = EyeSky;
        }
    }
}
