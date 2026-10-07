using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RemixNetworking{
    public static class WebUtilities
    {
        public static IEnumerator DownloadTextFromURL(string url, System.Action<string> onComplete)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("URL not set.");
                onComplete?.Invoke(null);
                yield break;
            }
    
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
    
                #if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
                #else
                if (request.isNetworkError || request.isHttpError)
                #endif
                {
                    Debug.Log("Failed to download text: " + request.error);
                    onComplete?.Invoke(null);
                }
                else
                {
                    onComplete?.Invoke(request.downloadHandler.text);
                }
            }
        }
    
        public static IEnumerator GetRemixLinks(System.Action<List<string>> onComplete)
        {
            List<string> urls = new List<string>()
            {
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/GenLinks/GTHDiscordLink.txt",
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/GenLinks/GTHADiscordLink.txt",
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/ApplabPage.txt",
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/GenLinks/ItchPage.txt",
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/GenLinks/GitLink.txt"
            };
    
            List<string> results = new List<string>();
    
            foreach (var url in urls)
            {
                using (UnityWebRequest req = UnityWebRequest.Get(url))
                {
                    yield return req.SendWebRequest();
                    results.Add(req.downloadHandler.text);
                }
            }
        
            onComplete?.Invoke(results);
        }
    
    }
}