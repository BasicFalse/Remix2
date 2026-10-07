using GorillaLocomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class EventHelper : MonoBehaviour
{
    [Header("EventSetter")]
    public bool stopsEvent;
    public bool stopTheEvent;

    [Header("EventDetector")]
    public bool isDetector;
    public bool finderOuter;
    public bool weekActive;
    public bool dayActive;
    public bool hourActive;
    public Text EventText;
    public ActivateOnDate dateThing;
    public GameObject[] EnableOnEvent;
    public GameObject[] DisableOnEvent;

    [Header("GameObject")]
    public GameObject GObject;
    public bool isGO;
    public bool setActiveness;
    public bool GOActiveness;

    [Header("Audio")]
    public bool isAudio;
    public bool playAudio;
    public bool unmuteAudio;
    public bool muteAudio;
    public bool useFloatMute;
    public float floatDecrement;
    public float soundPercent;
    public AudioClip audioSound;
    public AudioSource audioSource;

    [Header("SkySet")]
    public bool isSky;
    public bool isSkyPulse;
    public bool changeSky;
    public Material skySwap;
    public float exposure;

    [Header("Lighting")]
    public bool isLighting;
    public bool changeLighting;
    public Color setColor;

    [Header("Animation")]
    public bool isAnimation;
    public bool startAnimation;
    public Animator animationThing;

    [Header("RigidBody")]
    public Rigidbody theRigidbody;
    public bool isRigidbody;
    public bool setGravity;
    public bool useGravity;
    public bool setMass;
    public float RBMass;

    [Header("Monster")]
    public bool isMonster;
    public bool doMonst;
    public List<EnemyController> ECs;
    public List<Jumpscare2Point2> JSs;
    public bool activeStateMons;

    private void Update()
    {
        if(isAudio && playAudio)
        {
            audioSource.Play();
        }
        if (isAudio && muteAudio)
        {
            if (useFloatMute)
            {
                soundPercent = soundPercent - floatDecrement;
                audioSource.volume = soundPercent;
            }
            else
            {
                audioSource.mute = true;
            }
        }
        else
        {
            if (isAudio && unmuteAudio)
            {
                audioSource.mute = false;
            }
        }

        if (isGO)
        {
            if (setActiveness)
            {
                GObject.SetActive(GOActiveness);
            }
        }

        if (isSky && changeSky&&changeSky)
        {
            UnityEngine.RenderSettings.skybox = skySwap;
        }
        if (isSky && isSkyPulse && changeSky)
        {

            skySwap.SetFloat("_Exposure", exposure);
        }

        if(isLighting && changeLighting)
        {
            UnityEngine.RenderSettings.ambientLight = setColor;
        }

        if(stopsEvent && stopTheEvent)
        {
            LiveEventManager.instance.isPlaying = false;
        }

        if(isAnimation && startAnimation)
        {
            startAnimation = false;
            if (animationThing.enabled == true)
            {
                animationThing.enabled = false;
            }
            else
            {
                animationThing.enabled = true;
            }
        }

        if (isRigidbody)
        {
            if (setGravity)
            {
                theRigidbody.useGravity = useGravity;
            }
            if (setMass)
            {
                theRigidbody.mass = RBMass;
            }
        }

        if (isMonster)
        {
            if (doMonst)
            {
                foreach(EnemyController EC in ECs)
                {
                    EC.enabled = activeStateMons;
                }
                foreach(Jumpscare2Point2 JS in JSs)
                {
                    JS.enabled = activeStateMons;
                }
            }
        }

        if (isDetector)
        {
            if (LiveEventManager.instance.inSession)
            {
                if (finderOuter)
                {
                    if (LiveEventManager.instance.isPlaying)
                    {
                        EventText.text = "EVENT IS PLAYING";
                        EventText.color = Color.red;
                        foreach (GameObject thing in EnableOnEvent)
                        {
                            thing.SetActive(true);
                        }
                        foreach (GameObject thing in DisableOnEvent)
                        {
                            thing.SetActive(false);
                        }
                    }
                    else
                    {
                        EventText.color = Color.magenta;
                        if (dateThing.formattedActivation == "EventPassed" || dateThing.formattedActivation == "")
                        {
                            EventText.text = "";
                        }
                        else
                        {
                            if (weekActive && dateThing.lessThanOneWeek || dayActive && dateThing.lessThanOneDay || hourActive && dateThing.lessThanOneHour)
                            {
                                EventText.text = dateThing.formattedActivation + "\nUntil Event";
                            }
                        }
                        foreach (GameObject thing in DisableOnEvent)
                        {
                            thing.SetActive(true);
                        }
                        foreach (GameObject thing in EnableOnEvent)
                        {
                            thing.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                EventText.enabled = false;
            }
            if (weekActive && dateThing.lessThanOneWeek)
            {
                foreach(GameObject thing in EnableOnEvent)
                {
                    thing.SetActive(true);
                }
                foreach(GameObject thing in DisableOnEvent)
                {
                    thing.SetActive(false);
                }
            }
            if (dayActive && dateThing.lessThanOneDay)
            {
                foreach (GameObject thing in EnableOnEvent)
                {
                    thing.SetActive(true);
                }
                foreach (GameObject thing in DisableOnEvent)
                {
                    thing.SetActive(false);
                }
            }
            if (hourActive && dateThing.lessThanOneHour)
            {
                foreach (GameObject thing in EnableOnEvent)
                {
                    thing.SetActive(true);
                }
                foreach (GameObject thing in DisableOnEvent)
                {
                    thing.SetActive(false);
                }
            }
        }
    }
}
