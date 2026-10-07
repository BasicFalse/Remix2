using TMPro;
using UnityEngine;

[ExecuteAlways]
public class StepTextSetet : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public string[] stepText = new string[] {};
    public int currentStep = 0;

    void Update()
    {
        if (!tmp) return;
        if (stepText.Length == 0) return;
        if (currentStep < 0 || currentStep >= stepText.Length) return;

        tmp.text = stepText[currentStep];
    }
}
