using UnityEngine;

[CreateAssetMenu(fileName = "StartupManager", menuName = "System/StartupManager")]
public class StartupManager : ScriptableObject
{   
    private bool disallowSpawnOveride = false;
    public bool CanSpawnHaveOveride()
    {
        return !disallowSpawnOveride;
    }
    public void DisallowSpawnOverride()
    {
        disallowSpawnOveride = true;
    }
}
