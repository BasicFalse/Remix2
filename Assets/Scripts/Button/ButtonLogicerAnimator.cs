using System.Collections.Generic;
using UnityEngine;

public class ButtonLogicerAnimator : ButtonLogicer
{
    public Animator animControllerOther;
    [Header("--------\nAnimation Trigger Settings\n")]
    public string AOTrueStr;
    public string AOFalseStr;

    void Update()
    {
        if (buttons.Count == 0)
            return;

        // Extract button states
        List<bool> states = new List<bool>();
        foreach (Button button in buttons)
        {
            states.Add(button.pressed);
        }

        // Perform logic operations
        bool anyPressed = states.Contains(true);
        bool allPressed = !states.Contains(false);
        int pressedCount = states.FindAll(state => state).Count;
        buttonsPressed = pressedCount;

        even = buttonsPressed % 2 == 0;
        odd = !even;

        CalculateButtonSituation();
    }

    public new void CalculateButtonSituation()
    {
        if (buttonsPressed == 0)
        {
            sXor = false;
            sNor = true;
            sAnd = false;
            sNand = true;
            sOr = false;
        }
        else if (buttonsPressed > 0)
        {
            if (buttonsPressed < totalButtons)
            {
                sXor = true;
                sNor = false;
                sAnd = false;
                sNand = true;
                sOr = true;
            }
            else if (buttonsPressed == totalButtons)
            {
                sXor = false;
                sNor = false;
                sAnd = true;
                sNand = false;
                sOr = true;
            }
            else
            {
                Debug.Log("hello. there was a calc error. please help me. ooooaooaooaoaoa");
            }
        }
        if (invertYor)
        {
            if (even)
            {
                sYor = true;
            }
            else
            {
                sYor = false;
            }
        }
        else
        {
            if (even)
            {
                sYor = false;
            }
            else
            {
                sYor = true;
            }
        }
        Think();
    }

    public new void Think()
    {
        if (Xor == true)
        {
            if (Xor == sXor && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (Xor != sXor && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
        else if (Nor == true)
        {
            if (Nor == sNor && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (Nor != sNor && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
        else if (And == true)
        {
            if (And == sAnd && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (And != sAnd && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
        else if (Nand == true)
        {
            if (Nand == sNand && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (Nand != sNand && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
        else if (Or == true)
        {
            if (Or == sOr && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (Or != sOr && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
        else if (Yor == true)
        {
            if (Yor == sYor && !triggered)
            {
                animControllerOther.SetTrigger(AOTrueStr);
                triggered = true;
            }
            else if (Yor != sYor && triggered)
            {
                animControllerOther.SetTrigger(AOFalseStr);
                triggered = false;
            }
        }
    }
}
