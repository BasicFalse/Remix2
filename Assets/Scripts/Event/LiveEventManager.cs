using GorillaNetworking;
using GorillaRigging;
using UnityEngine;
using UnityEngine.Playables;

public class LiveEventManager : MonoBehaviour
{
    public static volatile LiveEventManager instance;

    public bool startManually;
    public bool inSession;
    public bool cannotPlay;
    public bool canPlay;
    public bool isWaiting;
    public bool isPlaying;
    public bool isSet;
    public bool playLatch;
    public bool didMute;
    public bool didEvent;

    public PlayableDirector PDTL;
    public Animator eventAnimaton;
    //public AnimatorController eventAnimatorController;

    public string animClauseToCall;

    public GameObject[] eventObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] array = eventObjects;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(value: false);
        }
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }

        if (canPlay)
        {
            isWaiting = true;
            Invoke("PlayEvent", 7f);
        }

        if (startManually && inSession)
        {
            cannotPlay = false;
            PlayEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (eventAnimaton == null || eventAnimatorController == null)
        //{
        //    cannotPlay = true;
        //}
        //else
        //{
        //    cannotPlay = false;
        //}


        if (!isPlaying && isSet && playLatch)
        {
            didEvent = true;
            isSet = false;
            playLatch = false;
        }

        if (canPlay && !isWaiting && !isPlaying)
        {
            PlayEvent();
        }

        if (isPlaying && !isSet)
        {
            if(eventAnimaton != null)
            {
                eventAnimaton.enabled = true;
            }
            if (PDTL != null)
            {
                PDTL.Play();
            }
            foreach (GameObject GO in eventObjects)
            {
                GO.SetActive(true);
            }
            isSet = true;
        }

        if(isPlaying && isSet && GorillaComputer.instance != null && GorillaComputer.instance.isInRoom)
        {
            if (GorillaComputer.instance.muteAllDuringEvents)
            {
                
                if (PhotonRoomManagerMain.instance != null)
                {
                    foreach (RigController child in PhotonRoomManagerMain.instance.rigs)
                    {
                        child.muted = true;
                        child.mutedTemp = true;
                    }
                    didMute = true;
                }
            }
            if (GorillaComputer.instance.muteAllDuringEvents && didMute)
            {
                foreach (RigController child in PhotonRoomManagerMain.instance.rigs)
                {
                    if (child.mutedTemp)
                    {
                        child.muted = false;
                    }
                }
                didMute = false;
            }
        }
        if (!isPlaying && didEvent)
        {
            if(GorillaComputer.instance.isInRoom)
            foreach (RigController child in PhotonRoomManagerMain.instance.rigs)
            {
                if (child.mutedTemp)
                {
                    child.muted = false;
                }
            }
        }
    }

    public void PlayEvent()
    {
        if (!cannotPlay && inSession)
        {
            isPlaying = true;
            playLatch = true;
        }
    }

    public void StartEventAnimation(string animClause)
    {
    }
}
