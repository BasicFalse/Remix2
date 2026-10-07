using System.Collections;
using RemixNetworking;
using Unity.Mathematics;
using UnityEngine;

public class RemixImages : MonoBehaviour
{
    public string fetchedText = "";

    public bool gotText = false;

    public bool3 status;

    public const string GitImagesLink = "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/Remix2Gen/RemixImages.txt";

    void Start()
    {
        status.x = true;
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        WebUtilities.DownloadTextFromURL(GitImagesLink, resultList =>
        {
                fetchedText = resultList; 
        });

        status.y = true;

        while (!gotText)
        {
            status.z = true;
            if(fetchedText != "")
                gotText = true;
            yield return null;
        }
    }
}
