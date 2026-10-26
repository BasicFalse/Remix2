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
