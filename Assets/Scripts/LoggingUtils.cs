using System;
using UnityEngine;

namespace RemixLogging{
    [Serializable]
    public struct LogDisplay
    {
        public string content;
        public string stack;
        public float lifetime;
        public int severity;
    };

    public static class LoggingStatics
    {
        public static readonly string[] noLogers = new string[]
        {
            "Audio source failed to initialize audio spatializer. An audio spatializer is specified in the audio project settings, but the associated plugin was not found or initialized properly. Please make sure that the selected spatializer is compatible with the target.",
            "dll file in: Not a directory",
            "movement check failed! we had to use the backup raycast. investigate the conditions that caused this to happen",
            "Look rotation viewing vector is zero"
        };
    }

    public static class LoggingConversion{
        public static int LogTypeToInt(LogType lt)
        {
            return lt switch
            {
                LogType.Log => 0,
                LogType.Warning => 1,
                LogType.Assert => 2,
                LogType.Error => 3,
                LogType.Exception => 4,
                _ => -1,
            };
        }
        public static string IntToRichStringColor(int type)
        {
            return type switch
            {
                0 => "<color=white>",
                1 => "<color=yellow>",
                2 => "<color=orange>",
                3 => "<color=red>",
                4 => "<color=pink>",
                _ => "<color=green>",
            };
        }
    }
}
