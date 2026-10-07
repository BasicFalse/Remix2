using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingTree : RemixScript
{
    public bool alive = true;

    [Header("Breathing Settings")]
    public float breathAmplitude = 0.2f;       // Scale change range
    public float breathFrequency = 0.3f;       // Breaths per second (~1 breath per 3s)
    public float breathOffset = 1f;            // Base scale multiplier
    public float holdDuration = 1f;            // Time to hold breath
    public float cutoffThreshold = 0.01f;      // Threshold to trigger holding

    public Vector3 baseScale = Vector3.one;
    public GameObject thisTree;

    [Header("Breath Sounds")]
    public AudioClip inhaleSound;
    public AudioClip exhaleSound;
    public List<AudioSource> ASources;

    private float time;
    private bool holding;
    private bool holdingAtTop;
    private float lastSine = 0f;

    public override void StartExt()
    {
        if (thisTree == null)
            thisTree = gameObject;

        baseScale = thisTree.transform.localScale;
        time = 0f;
        holding = false;
    }

    void Update()
    {
        if (!alive || holding) return;

        time += Time.deltaTime;

        // Sine wave: -1 to 1
        float sine = Mathf.Sin(time * breathFrequency * 2f * Mathf.PI);

        // Determine breath phase by slope of sine
        float delta = sine - lastSine;

        // Play inhale or exhale sound when passing midpoint
        if (lastSine < 0 && sine >= 0 && inhaleSound != null)
        {
            BreathSound(1);
        }
        else if (lastSine > 0 && sine <= 0 && exhaleSound != null)
        {
            BreathSound(2);
        }

        lastSine = sine;

        // Scale breathing
        float scaleMultiplier = breathOffset + sine * breathAmplitude;
        thisTree.transform.localScale = baseScale * scaleMultiplier;

        // Trigger breath hold
        if (Mathf.Abs(sine - 1f) < cutoffThreshold && !holdingAtTop)
        {
            StartCoroutine(HoldBreath(true));
        }
        else if (Mathf.Abs(sine + 1f) < cutoffThreshold && holdingAtTop)
        {
            StartCoroutine(HoldBreath(false));
        }
    }

    private IEnumerator HoldBreath(bool atTop)
    {
        holding = true;
        holdingAtTop = atTop;

        yield return new WaitForSeconds(holdDuration);
        holding = false;
    }

    private void BreathSound(int type)
    {
        switch (type)
        {
            case 1:
                foreach (AudioSource AS in ASources)
                {
                    AS.PlayOneShot(inhaleSound);
                }
                break;

            case 2:
                foreach (AudioSource AS in ASources)
                {
                    AS.PlayOneShot(exhaleSound);
                }
                break;

            default:
                Debug.Log("areyoufr");
                break;
        }
    }
}
