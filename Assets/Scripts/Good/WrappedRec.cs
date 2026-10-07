using System.Collections;
using TMPro;
using UnityEngine;

public class WrappedRec : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1);
    public RecordManager recordManager;
    public TextMeshProUGUI Text;
    public GameObject Buttons;
    public Animator animator;
    public bool wasDoingStuff = false;

    public void StartWrappRec()
    {
        StartCoroutine(WrapRecCoroutine());
    }

    IEnumerator WrapRecCoroutine()
    {
        Text.text = "3";
        yield return _waitForSeconds1;
        Text.text = "2";
        yield return _waitForSeconds1;
        Text.text = "";
        yield return _waitForSeconds1;
        Buttons.SetActive(false);
        animator.SetTrigger("Start");
        //animator.StartPlayback();
        recordManager.StartRecording();
        Buttons.SetActive(false);
        wasDoingStuff = true;
    }

    public void Update()
    {
        if(wasDoingStuff){
        if (IsAnyOneShotPlaying(animator))
        {
            return;
        }
        {
            recordManager.StopRecording();
            Buttons.SetActive(true);
            wasDoingStuff = false;
        }}
    }

    bool IsAnyOneShotPlaying(Animator animator)
    {
        for (int i = 0; i < animator.layerCount; i++)
        {
            var state = animator.GetCurrentAnimatorStateInfo(i);

            if (!state.loop &&
                (state.normalizedTime < 1f || animator.IsInTransition(i)))
            {
                return true;
            }
        }
        return false;
    }
}
