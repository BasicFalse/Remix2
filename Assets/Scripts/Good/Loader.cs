using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : RemixScript
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(1);
    }
}
