using System.Collections.Generic;
using UnityEngine;

public class ModularOpenSignController : RemixScript
{
    [System.Serializable]
    public class SignElement
    {
        public Light lightComponent;
        public MeshRenderer meshRenderer;
        public int materialElementIndex;
        public Material litMaterial;
        public Material unlitMaterial;
    }

    public List<SignElement> signElements;
    public float switchInterval = 0.5f;
    public float blinkInterval = 0.2f;
    public int blinkCount = 3;
    public Transform Player;
    public float LDModeDistance = 25f;
    public Light combinedLight;

    private int currentIndex;
    private float timer;
    private bool isBlinking;
    private int currentBlink;
    private bool allOffBeforeBlinking;
    private bool waitingBeforeAllOff;
    public bool LDMode;

    public override void StartExt()
    {
        if (signElements.Count == 0)
        {
            Debug.LogError("Sign elements are not configured!");
            return;
        }

        ResetSign();
        currentIndex = 0;
        isBlinking = false;
        currentBlink = 0;
        allOffBeforeBlinking = false;
        waitingBeforeAllOff = false;

        if (combinedLight != null)
            combinedLight.enabled = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(Player.position, transform.position);
        LDMode = distance > LDModeDistance;

        timer += Time.deltaTime;

        if (!isBlinking && !allOffBeforeBlinking && !waitingBeforeAllOff)
        {
            if (timer >= switchInterval)
            {
                timer = 0f;
                ActivateElement(currentIndex, true);

                currentIndex++;

                if (currentIndex >= signElements.Count)
                {
                    currentIndex = 0;
                    waitingBeforeAllOff = true;
                    timer = 0f;
                }
            }
        }
        else if (waitingBeforeAllOff)
        {
            if (timer >= switchInterval)
            {
                waitingBeforeAllOff = false;
                allOffBeforeBlinking = true;
                timer = 0f;
            }
        }
        else if (allOffBeforeBlinking)
        {
            ResetSign();
            allOffBeforeBlinking = false;
            isBlinking = true;
            currentBlink = 0;
        }
        else if (isBlinking)
        {
            if (timer >= blinkInterval)
            {
                timer = 0f;
                bool isOn = currentBlink % 2 == 0;
                SetAllElements(isOn);

                currentBlink++;
                if (currentBlink >= blinkCount * 2)
                {
                    isBlinking = false;
                    ResetSign();
                }
            }
        }
    }

    private void ActivateElement(int index, bool isOn)
    {
        if (index >= 0 && index < signElements.Count)
        {
            SignElement element = signElements[index];

            Material[] materials = element.meshRenderer.materials;
            if (element.materialElementIndex >= 0 && element.materialElementIndex < materials.Length)
            {
                materials[element.materialElementIndex] = isOn ? element.litMaterial : element.unlitMaterial;
                element.meshRenderer.materials = materials;
            }

            if (element.lightComponent != null)
                element.lightComponent.enabled = isOn;
        }

        UpdateCombinedLight();
    }

    private void SetAllElements(bool isOn)
    {
        foreach (SignElement element in signElements)
        {
            Material[] materials = element.meshRenderer.materials;

            if (element.materialElementIndex >= 0 && element.materialElementIndex < materials.Length)
            {
                materials[element.materialElementIndex] = isOn ? element.litMaterial : element.unlitMaterial;
                element.meshRenderer.materials = materials;
            }

            if (element.lightComponent != null)
                element.lightComponent.enabled = isOn;
        }

        UpdateCombinedLight();
    }

    private void ResetSign()
    {
        foreach (SignElement element in signElements)
        {
            Material[] materials = element.meshRenderer.materials;

            if (element.materialElementIndex >= 0 && element.materialElementIndex < materials.Length)
            {
                materials[element.materialElementIndex] = element.unlitMaterial;
                element.meshRenderer.materials = materials;
            }

            if (element.lightComponent != null)
            {
                element.lightComponent.enabled = false;
                element.lightComponent.gameObject.SetActive(!LDMode);
            }
        }

        UpdateCombinedLight();
    }

    private void UpdateCombinedLight()
    {
        if (combinedLight == null) return;

        int onCount = 0;
        float intens;
        foreach (var element in signElements)
        {
            if (element.lightComponent != null && element.lightComponent.enabled)
                onCount++;
        }

        intens = onCount;

        if (onCount > 0)
        {
            combinedLight.enabled = true;
            combinedLight.intensity = intens/5; // Scales brightness with lights on
        }
        else
        {
            combinedLight.enabled = false;
        }
    }
}
