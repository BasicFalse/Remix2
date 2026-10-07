using UnityEngine;
using UnityEngine.UI;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        public Text outText;
        static string myLog = "";
        private string output;
        private string stack;
        private static readonly string[] noLogers = new string[]
        {
            "Audio source failed to initialize audio spatializer. An audio spatializer is specified in the audio project settings, but the associated plugin was not found or initialized properly. Please make sure that the selected spatializer is compatible with the target.",
            "dll file in: Not a directory",
            "movement check failed! we had to use the backup raycast. investigate the conditions that caused this to happen",
            "Look rotation viewing vector is zero"
        };

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            if(!AnyMatch(output))
            myLog = output + "\n" + myLog;
        }
        private bool AnyMatch(string s)
        {
            foreach(string s2 in noLogers)
            {
                if(s == s2)
                {
                    return true;
                }
            }
            return false;
        }
        void Update()
        {
            outText.text = myLog;
        }
    }
}