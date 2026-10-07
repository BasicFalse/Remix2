using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class ButtonAnimationTriggerInt : Button
{   //mybrainisfried

    public Animator animControllerOther;
    [Header("--------\nAnimation Trigger Settings\n")]
    public string AOTrueStr;
    public string AOFalseStr;

    private new void Update()
    {
        if (test && !isCooldown)
        {
            test = false;
            StartCoroutine(PlayAnimation());
        }
    }

    private new void OnTriggerEnter(Collider other)
    {
        if (!isCooldown)
        {
            StartCoroutine(PlayAnimation2());
        }
    }

    public IEnumerator PlayAnimation2()
    {
        if (animController != null)
        {
            isCooldown = true;

            if (Lock)
            {
                if (!pressed)
                {
                    animController.SetTrigger("ButtonAnimLock");
                    animControllerOther.SetTrigger(AOTrueStr);
                }
                else
                {
                    Debug.Log("nah");
                }
            }
            else if (Press)
            {
                animController.SetTrigger("ButonAnimPress");
                animControllerOther.SetTrigger(AOTrueStr);
                yield return new WaitForSeconds(pressTime);
                animControllerOther.SetTrigger(AOFalseStr);
            }
            else if (Toggle)
            {
                if (ToggleLock)
                {
                    animController.SetTrigger("ButonAnimToggleIn");
                    ToggleLock = false;
                    animControllerOther.SetTrigger(AOFalseStr);
                }
                else
                {
                    animController.SetTrigger("ButonAnimToggleOut");
                    ToggleLock = true;
                    animControllerOther.SetTrigger(AOTrueStr);
                }
            }

            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
        }
        else
        {
            Debug.Log("give me anim controller");
        }
    }
}
