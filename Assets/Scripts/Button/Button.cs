using System.Collections;
using UnityEngine;

public class Button : MonoBehaviour
{   //mybrainisfried

    public Animator animController;

    [Header("--------\nScript Managed Debug\n")]
    public bool test;
    public bool pressed;
    public bool depressed;
    public bool ToggleLock;

    [Header("--------\nType\n")]
    public bool Lock;
    public bool Press;
    public bool Toggle;

    [Header("--------\nSettings\n")]
    public float pressTime = .5f;
    public float cooldownTime = .5f;
    public bool isCooldown = false;
    public Collider customButtonTrigger;

    private void Start()
    {
        if (customButtonTrigger != null)
        {
            customButtonTrigger.isTrigger = true;
        }
        if (animController == null)
        {
            animController = GetComponent<Animator>();
        }
    }

    public void Update()
    {
        if (test && !isCooldown)
        {
            test = false;
            StartCoroutine(PlayAnimation());
        }
        if (pressed)
        {
            depressed = false;
        }
        else
        {
            depressed = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isCooldown)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    public IEnumerator PlayAnimation()
    {
        
            isCooldown = true;

            if (Lock)
            {
                if (!pressed)
                {
                    pressed = true;
                }
                else
                {
                    Debug.Log("nah");
                }
            }
            else if (Press)
            {
                pressed = true;
                yield return new WaitForSeconds(pressTime);
                pressed = false;
            }
            else if (Toggle)
            {
                if (ToggleLock)
                {
                    ToggleLock = false;
                    pressed = false;
                }
                else
                {
                    ToggleLock = true;
                    pressed = true;
                }
            }

            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
    }
}
