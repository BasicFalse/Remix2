using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public string[] pointers;
    public bool isOn;

    public void OnTriggerEnter(Collider other)
    {
        AudioManager.sound.ToggleGroup(pointers, isOn);
    }

    public void Start()
    {
        if(gameObject.GetComponent<MeshRenderer>() != null)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
