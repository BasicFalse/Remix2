using UnityEngine;
using UnityEngine.UI;
using TMPro; // Only needed if using TextMeshPro
using System.Collections;
using UnityEngine.Networking;

public class GitHubTextLoader : RemixScript
{
    [Header("Raw GitHub File URL")]
    public string githubRawUrl = "https://raw.githubusercontent.com/user/repo/branch/file.txt";

    [Header("UI Text Target")]
    public Text unityTextComponent;
    public TextMeshProUGUI tmpTextComponent;

    void Start()
    {
        StartCoroutine(DownloadTextFromGitHub());
    }


    [ContextMenu("Redo Download")]
    IEnumerator DownloadTextFromGitHub()
    {
        if (string.IsNullOrEmpty(githubRawUrl))
        {
            Debug.LogError("GitHub URL is not set.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(githubRawUrl))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.Log("Failed to download text: " + request.error);
                if (unityTextComponent != null)
                    unityTextComponent.text = "DATA DOWNLOAD ERROR // ARE YOU OFFLINE?";

                if (tmpTextComponent != null)
                    tmpTextComponent.text = "DATA DOWNLOAD ERROR // ARE YOU OFFLINE?";
            }
            else
            {
                string downloadedText = request.downloadHandler.text;

                if (unityTextComponent != null)
                    unityTextComponent.text = downloadedText;

                if (tmpTextComponent != null)
                    tmpTextComponent.text = downloadedText;

                Debug.Log("Text successfully loaded from GitHub.");
            }
        }
    }
}
