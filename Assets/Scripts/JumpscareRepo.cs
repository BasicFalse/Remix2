using System;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpscareRepo", menuName = "DataHolders/JumpscareRepo")]
public class JumpscareRepo : ScriptableObject
{
    [SerializeField]
    public JumpscareVisual[] jumpscareVisuals;

    public GameObject GetJumpscarePrefab(string id)
    {
        for (int i = 0; i < jumpscareVisuals.Length; i++)
        {
            if (jumpscareVisuals[i].jumpscareID == id)
            {
                return jumpscareVisuals[i].jumpscarePrefab;
            }
        }
        return null;
    }
}


[Serializable]
public struct JumpscareVisual
{
    [SerializeField]
    public string jumpscareID;
    [SerializeField]
    public GameObject jumpscarePrefab;
}