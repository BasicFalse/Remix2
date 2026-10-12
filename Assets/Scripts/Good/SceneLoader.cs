using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SceneLoader : RemixScript
{
    private static readonly WaitForSeconds _waitForSeconds5 = new(5f);
    public bool isLoading;
    public bool canDo;
    public Slider loadBar;
    public Text loadText;
    public float dotDotDot;
    public int sceneToLoad;

    void Update()
    {
        string diagnostics = "/storage/emulated/0/Android/data/" + Application.identifier + "/files/Mods";
        if (Directory.Exists(diagnostics))
        {
            canDo = false;
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "b53on25b-2942-2n8s-jia0-wh3u2p40amd8";
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat = "ahe901jd-nv92-aci3-m4io-na93j58am31k";
            Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }
        else
        {
            canDo = true;
        }

        if (!isLoading && canDo)
        {
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }

            dotDotDot -= Time.deltaTime * 3f;
        if (dotDotDot <= 0)
            dotDotDot = 10;

        if (dotDotDot >= 7f)
            loadText.text = "Loading .";
        else if (dotDotDot >= 4f)
            loadText.text = "Loading . .";
        else
            loadText.text = "Loading . . .";
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        isLoading = true;
        yield return _waitForSeconds5;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadBar.value = progress;

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        isLoading = false;
        canDo = false;
    }
}
