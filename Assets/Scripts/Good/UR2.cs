using UnityEngine;

public class UR2 : RemixScript
{
    private void OnTriggerEnter(Collider other)
    {
        Application.OpenURL("https://discord.com/invite/HG5BvNEzsE");
    }
}
