using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MxUnikit.Log
{
    public static class MxLog
    {
        private static MxLogConfig Config => MxLogConfig.Instance;

        private static bool IsEnabled => Config == null || Config.IsEnabled;
        private static bool LogStackTraceForExceptions => Config == null || Config.LogStackTraceForExceptions;

        #region Public API

        public static void L(string message) => LogInternal(message, null, LogType.Log);

        public static void W(string message) => LogInternal(message, null, LogType.Warning);

        public static void E(string message) => LogInternal(message, null, LogType.Error);

        public static void Ex(Exception ex) => LogException(ex, null, null);
        public static void Ex(string message, Exception ex) => LogException(ex, message, null);

        #endregion

        #region Internal

        private static void LogInternal(string message, MxLogCategory category, LogType type)
        {
            if (!IsEnabled) return;

            (string className, string methodName) = GetCallerInfo();
            MxLogCategory cat = category ?? DetectCategory(className, methodName, message);

            if (Config != null && !Config.IsCategoryEnabled(cat)) return;

            string color = Config?.GetCategoryColor(cat);
            bool hasColor = !string.IsNullOrEmpty(color);

            string formatted = hasColor
                ? $"[<color={color}>{className}</color>] - <color={color}>{methodName}()</color> : {message}"
                : $"[{className}] - {methodName}() : {message}";

            switch (type)
            {
                case LogType.Log:
                    Debug.Log(formatted);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(formatted);
                    break;
                case LogType.Error:
                    Debug.LogError(formatted);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(formatted);
                    break;
                case LogType.Exception:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static (string className, string methodName) GetCallerInfo()
        {
            StackTrace stackTrace = new StackTrace(3, false);
            StackFrame frame = stackTrace.GetFrame(0);

            if (frame == null) return ("Unknown", "Unknown");

            MethodBase method = frame.GetMethod();
            if (method == null) return ("Unknown", "Unknown");

            string className = method.DeclaringType?.Name ?? "Unknown";
            string methodName = method.Name;

            if (!methodName.StartsWith("<")) return (className, methodName);

            int endIndex = methodName.IndexOf('>');
            if (endIndex > 1)
            {
                methodName = methodName[1..endIndex];
            }

            return (className, methodName);
        }

        private static MxLogCategory DetectCategory(string className, string methodName, string message)
        {
            if (string.IsNullOrEmpty(className) && string.IsNullOrEmpty(methodName) && string.IsNullOrEmpty(message))
            {
                return MxLogCategory.Default;
            }

            string source = $"{className} {methodName} {message}".ToLowerInvariant();
            string[] words = source.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                MxLogCategory category = Config?.DetectCategoryFromKeyword(word);
                if (category != null && category != MxLogCategory.Default)
                {
                    return category;
                }
            }

            return MxLogCategory.Default;
        }

        private static void LogException(Exception ex, string additionalMessage, MxLogCategory category)
        {
            if (!IsEnabled || ex == null) return;

            (string className, string methodName) = GetCallerInfo();
            MxLogCategory cat = category ?? DetectCategory(className, methodName, additionalMessage);

            if (Config != null && !Config.IsCategoryEnabled(cat)) return;

            string color = Config?.GetCategoryColor(cat);
            bool hasColor = !string.IsNullOrEmpty(color);

            string header = hasColor
                ? $"[<color={color}>{className}</color>] - <color={color}>{methodName}()</color>"
                : $"[{className}] - {methodName}()";

            string exceptionType = ex.GetType().Name;
            string exceptionMessage = ex.Message;

            string formatted = additionalMessage != null
                ? $"{header} : {additionalMessage}\n<color=#FF0000>{exceptionType}</color>: {exceptionMessage}"
                : $"{header}\n<color=#FF0000>{exceptionType}</color>: {exceptionMessage}";

            if (LogStackTraceForExceptions && !string.IsNullOrEmpty(ex.StackTrace))
            {
                formatted += $"\n{ex.StackTrace}";
            }

            if (ex.InnerException != null)
            {
                formatted +=
                    $"\n---> Inner Exception: <color=#FF4500>{ex.InnerException.GetType().Name}</color>: {ex.InnerException.Message}";
                if (LogStackTraceForExceptions && !string.IsNullOrEmpty(ex.InnerException.StackTrace))
                {
                    formatted += $"\n{ex.InnerException.StackTrace}";
                }
            }

            Debug.LogError(formatted);
        }

        #endregion
    }
}
