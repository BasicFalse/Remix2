using Photon.Pun;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace UFunction
{
    public class Startup
    {
        private Startup() { Update(); }

        private static readonly string[] checks = { "MelonLoader.dll", "LemonLoader.dll" };

        private static bool isRunning = false;

        [RuntimeInitializeOnLoadMethod]
        public static async void Update()
        {
            if(Application.platform != RuntimePlatform.Android) { isRunning = true; }
            if (!isRunning)
            {
                //Debug.Log("boo");
                do
                {
                    isRunning = true;
                    SystemCheck();
                    await Task.Delay(100);
                } while (isRunning);
            }
        }

        private static void SystemCheck()
        {
            string diagnostics = "/storage/emulated/0/Android/data/" + Application.identifier + "/files/Mods";

            if (Directory.Exists(diagnostics))
            {
                PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "b53on25b-2942-2n8s-jia0-wh3u2p40amd8";
                PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat = "ahe901jd-nv92-aci3-m4io-na93j58am31k";
                Application.Quit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(126);
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                string Path = $"/storage/emulated/0/Android/data/{Application.identifier}/files";
                string[] AllDlls = Directory.GetFiles(Path, "*.dll", SearchOption.AllDirectories);

                foreach (string Dll in AllDlls)
                {
                    if (!Dll.Contains("/il2cpp/") && !Dll.Contains("/cache/"))
                    {
                        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "b53on25b-2942-2n8s-jia0-wh3u2p40amd8";
                        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat = "ahe901jd-nv92-aci3-m4io-na93j58am31k";
                        File.Delete(Dll);
                        Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
                        Utils.NativeError("Packet error: 0x80000000 | Undefined");
                        Application.Quit();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        System.Environment.Exit(126);
                        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        currentActivity?.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                            {                currentActivity.Call("finishAffinity");
                            }));
                                    }
                }
            }

            try
            {
                checkdll(Application.dataPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"dll file in: {e.Message}");
            }
        }

        static void checkdll(string directory)
        {
            foreach (string dllName in checks)
            {
                string[] foundDlls = Directory.GetFiles(directory, dllName, SearchOption.AllDirectories);

                foreach (string dll in foundDlls)
                {
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "b53on25b-2942-2n8s-jia0-wh3u2p40amd8";
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat = "ahe901jd-nv92-aci3-m4io-na93j58am31k";
                    File.Delete(dll);
                    Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
                    Utils.NativeError("Packet error: 0x80000000 | Undefined");
                    Application.Quit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    System.Environment.Exit(126);
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    currentActivity?.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                        {                currentActivity.Call("finishAffinity");
                        }));
                }
            }
        }
        
        public static void CallIt()
        {
            Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
            Utils.NativeError("Packet error: 0x80000000 | Undefined");
            Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(126);
            AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity?.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {currentActivity.Call("finishAffinity");
                }));
        }
    }
}