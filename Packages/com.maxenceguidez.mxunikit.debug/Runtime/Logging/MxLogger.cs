using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace MxUnikit.Debug.Logging
{
    public static class MxLogger
    {
        // --------------------------------------------------------------------------------------------
        private static bool _debug = true;

        // ----------------------------------------------------------------------------------------
        private enum LogType
        {
            Info,
            Warning,
            Error
        }

        // ----------------------------------------------------------------------------------------
        public static void L(string message)
        {
            Log(LogType.Info, message);
        }

        // ----------------------------------------------------------------------------------------
        public static void L(Color color, string message)
        {
            L($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }

        // ----------------------------------------------------------------------------------------
        public static void W(string message)
        {
            Log(LogType.Warning, message);
        }

        // ----------------------------------------------------------------------------------------
        public static void E(string message)
        {
            Log(LogType.Error, message);
        }

        // ----------------------------------------------------------------------------------------
        private static void Log(LogType type, string message)
        {
            if (!_debug) return;

            switch (type)
            {
                case LogType.Error:
                    UnityDebug.LogError(message);
                    break;
                case LogType.Warning:
                    UnityDebug.LogWarning(message);
                    break;
                case LogType.Info:
                default:
                    UnityDebug.Log(message);
                    break;
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}
