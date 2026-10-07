using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionData : MonoBehaviour
{
    public enum FunctionType
    {
        Nothing = 0,
        Recharge = 1
    }

    public FunctionType function;

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
    }
}
