
using UnityEditor;
using UnityEngine;

public class MissingScriptTool
{
    const string k_missingScriptsMenuFolder = "Tools/Missing Scripts/";

    [MenuItem(k_missingScriptsMenuFolder + "Find")]

    static void FindMissingScriptsMenuItem()
    {
        foreach (GameObject gameObject in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
        {
            foreach (Component component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log($"GameObject found with missing script {gameObject.name}", gameObject);
                    break;
                }
            }

        }

#pragma warning disable CS8321 // Local function is declared but never used
        [MenuItem(k_missingScriptsMenuFolder + "Delete")]

        static void DeleteMissingScriptsMenuItem()
        {
            foreach (GameObject gameObject in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                foreach (Component component in gameObject.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                        break;
                    }
                }
            }
        }
#pragma warning restore CS8321 // Local function is declared but never used
    }
}
    