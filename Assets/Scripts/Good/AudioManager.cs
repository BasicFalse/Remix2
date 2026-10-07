using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioGroup
    {
        public string identifier;
        public bool isOnByStart;
        public AudioSource[] AudioSources;

        public void ToggleSources(bool active)
        {
            for(int i =0; i < AudioSources.Length; i++)
            {
                AudioSources[i].enabled = active;
            }
        }
    }

    public static volatile AudioManager sound;

    public AudioGroup[] AudioGroups;

    public void Awake()
    {
        if(sound == null)
        {
            sound = this;
        }
        else
        {
            if(sound != this){Destroy(gameObject);}
        }
    }
    public void Start()
    {
        foreach(AudioGroup g in AudioGroups)
        {
            g.ToggleSources(g.isOnByStart);
        }
    }

    public void ToggleGroup(string[] pointers, bool toggle)
    {
        foreach(string point in pointers)
        {
            foreach(AudioGroup g in AudioGroups)
            {
                if(g.identifier == point)
                {
                    g.ToggleSources(toggle);
                }
            }
        }
    }
}
