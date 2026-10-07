using System.Collections.Generic;
using UnityEngine;

public class ButtonLogicer : MonoBehaviour
{
    [Header("Logic\n")]
    public bool Xor;
    public bool Yor;
    public bool Nor;
    public bool And;
    public bool Nand;
    public bool Or;

    [Header("--------\nSettings\n")]
    public List<Button> buttons;
    public bool invertYor;

    [Header("--------\nScript Managed\n")]
    public int buttonsPressed;
    public int totalButtons;
    public bool even;
    public bool odd;
    public bool triggered = false;
    public bool sXor;
    public bool sYor;
    public bool sNor;
    public bool sAnd;
    public bool sNand;
    public bool sOr;

    [Header("--------\nDebug\n")]
    public bool noButtonsPressed;
    public bool oneButtonPressed;
    public bool lessButMoreButtonsPressed;
    public bool allButtonsPressed;

    private void Start()
    {
        totalButtons = buttons.Count;

        int count = (Xor ? 1 : 0) + (Nor ? 1 : 0) + (And ? 1 : 0) + (Nand ? 1 : 0) + (Or ? 1 : 0) + (Yor ? 1 : 0);
        if (count > 1)
        {
            Debug.Log("nuh uh. not gonna do that. nah. think about the gas prices. nope. " + gameObject.name, gameObject);
        }
    }

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

        // Determine if buttonsPressed is even or odd
        even = (buttonsPressed % 2 == 0);
        odd = !even;

        CalculateButtonSituation();
    }

    public void CalculateButtonSituation()
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
                sYor = false;
            }
            else
            {
                sYor = true;
            }
        }
        else
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
        Think();
    }

    public void Think()
    {
        if (Xor == true)
        {
            if (Xor == sXor && !triggered)
            {
                triggered = true;
            }
            else if (Xor != sXor && triggered)
            {
                triggered = false;
            }
        }
        else if (Nor == true)
        {
            if (Nor == sNor && !triggered)
            {
                triggered = true;
            }
            else if (Nor != sNor && triggered)
            {
                triggered = false;
            }
        }
        else if (And == true)
        {
            if (And == sAnd && !triggered)
            {
                triggered = true;
            }
            else if (And != sAnd && triggered)
            {
                triggered = false;
            }
        }
        else if (Nand == true)
        {
            if (Nand == sNand && !triggered)
            {
                triggered = true;
            }
            else if (Nand != sNand && triggered)
            {
                triggered = false;
            }
        }
        else if (Or == true)
        {
            if (Or == sOr && !triggered)
            {
                triggered = true;
            }
            else if (Or != sOr && triggered)
            {
                triggered = false;
            }
        }
        else if (Yor == true)
        {
            if (Yor == sYor && !triggered)
            {
                triggered = true;
            }
            else if (Yor != sYor && triggered)
            {
                triggered = false;
            }
        }
    }
}
