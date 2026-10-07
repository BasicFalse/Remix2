using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;

[RequireComponent(typeof(MeshRenderer))]
public class LucyColor : MonoBehaviour
{
    public Material[] materialArray;
    public float cooldownDuration = 0.2f;
    public int RendIndex = 0;

    private int currentMaterialIndex = 0;
    private MeshRenderer meshRenderer;
    private bool isCooldown = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetMaterial(currentMaterialIndex);
    }

    private void Update()
    {
        if (!isCooldown && EasyInputs.GetSecondaryButtonDown(EasyHand.LeftHand))
        {
            // Cycle to the next material in the list
            currentMaterialIndex = (currentMaterialIndex + 1) % materialArray.Length;
            SetMaterial(currentMaterialIndex);

            // Start cooldown
            StartCooldown();
        }
    }

    private void SetMaterial(int index)
    {
        // Ensure the index is within bounds
        if (index >= 0 && index < materialArray.Length)
        {
            Material[] array = meshRenderer.materials;
            array[RendIndex] = materialArray[index];
            meshRenderer.materials = array;
        }
    }

    private void StartCooldown()
    {
        isCooldown = true;
        Invoke("ResetCooldown", cooldownDuration);
    }

    private void ResetCooldown()
    {
        isCooldown = false;
    }
}
