using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // Use TMPro if using TextMeshPro

public class PastebinTextFetcher : RemixScript
{
    public string pastebinURL = "https://pastebin.com/raw/your-pastebin-id"; // Replace with your raw Pastebin URL
    public Text uiText; // Use TextMeshProUGUI if using TextMeshPro
    public Text compareText; // Text component to compare with fetched text
    public Renderer targetRenderer; // Object whose material to change
    public Material offlineMaterial; // Material to apply when offline or if texts don't match
    public Material onlineMaterial; // Material to apply when texts match and online
    public List<GameObject> objectsToEnable = new List<GameObject>(); // List of objects to enable/disable

    private Material originalMaterial; // Store the original material for the second slot

    public override void StartExt()
    {
        // Store the original material at the second index at start
        if (targetRenderer != null && targetRenderer.materials.Length > 1)
        {
            originalMaterial = targetRenderer.materials[1];
        }

        StartCoroutine(FetchTextFromPastebin());
    }

    IEnumerator FetchTextFromPastebin()
    {
        UnityWebRequest request = UnityWebRequest.Get(pastebinURL);

        // Send the request
        yield return request.SendWebRequest();

        // Check if the request was successful
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Set text to the fetched data
            string fetchedText = request.downloadHandler.text;
            uiText.text = fetchedText;

            // Check if the fetched text matches the compareText content
            if (compareText != null && fetchedText != compareText.text)
            {
                // If texts don't match, enable the objects and set material to offline
                SetObjectsActiveState(true);
                SetMaterial(offlineMaterial);
            }
            else
            {
                // If texts match, disable the objects and set material to online
                SetObjectsActiveState(false);
                SetMaterial(onlineMaterial);
            }
        }
        else
        {
            // If offline or failed, display "OFFLINE" and set the offline material
            uiText.text = "OFFLINE";
            SetObjectsActiveState(false);
            SetMaterial(offlineMaterial);

            Debug.LogError("Failed to fetch text: " + request.error);
        }
    }

    private void SetObjectsActiveState(bool state)
    {
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }

    private void SetMaterial(Material material)
    {
        if (material != null && targetRenderer.materials.Length > 1 && targetRenderer.materials[1] != material)
        {
            Material[] materials = targetRenderer.materials;
            materials[1] = material;
            targetRenderer.materials = materials;
        }
    }
}