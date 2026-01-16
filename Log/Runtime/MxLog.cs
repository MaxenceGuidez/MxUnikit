using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MxUnikit.Log
{
    public enum MxLogCategory
    {
        Default,

        API,
        Audio,
        Debug,
        Event,
        Firebase,
        Game,
        Inputs,
        Inventory,
        Network,
        Player,
        Session,
        UI
    }

    public static class MxLog
    {
        private static MxLogConfig Config => MxLogConfig.Instance;

        private static bool IsEnabled => Config == null || Config.IsEnabled;
        private static bool LogStackTraceForExceptions => Config == null || Config.LogStackTraceForExceptions;

        private static readonly HashSet<MxLogCategory> _defaultDisabledCategories = new HashSet<MxLogCategory>();

        private static readonly Dictionary<MxLogCategory, string> _defaultColors = new Dictionary<MxLogCategory, string>
        {
            { MxLogCategory.API, "#00CED1" },
            { MxLogCategory.Audio, "#9370DB" },
            { MxLogCategory.Debug, "#FF6347" },
            { MxLogCategory.Event, "#DA70D6" },
            { MxLogCategory.Firebase, "#FFA500" },
            { MxLogCategory.Game, "#32CD32" },
            { MxLogCategory.Inputs, "#1E90FF" },
            { MxLogCategory.Inventory, "#FFD700" },
            { MxLogCategory.Network, "#4A90D9" },
            { MxLogCategory.Player, "#ADFF2F" },
            { MxLogCategory.UI, "#FF69B4" }
        };

        private static readonly Dictionary<string, MxLogCategory> _defaultKeywords = new Dictionary<string, MxLogCategory>
        {
            { "api", MxLogCategory.API },
            { "backend", MxLogCategory.API },
            { "http", MxLogCategory.API },
            { "request", MxLogCategory.API },
            { "audio", MxLogCategory.Audio },
            { "music", MxLogCategory.Audio },
            { "sound", MxLogCategory.Audio },
            { "debug", MxLogCategory.Debug },
            { "event", MxLogCategory.Event },
            { "eventbus", MxLogCategory.Event },
            { "publish", MxLogCategory.Event },
            { "subscribe", MxLogCategory.Event },
            { "firebase", MxLogCategory.Firebase },
            { "game", MxLogCategory.Game },
            { "device", MxLogCategory.Inputs },
            { "input", MxLogCategory.Inputs },
            { "inputs", MxLogCategory.Inputs },
            { "joystick", MxLogCategory.Inputs },
            { "key", MxLogCategory.Inputs },
            { "keyboard", MxLogCategory.Inputs },
            { "mouse", MxLogCategory.Inputs },
            { "touch", MxLogCategory.Inputs },
            { "accessory", MxLogCategory.Inventory },
            { "accessories", MxLogCategory.Inventory },
            { "inventory", MxLogCategory.Inventory },
            { "item", MxLogCategory.Inventory },
            { "items", MxLogCategory.Inventory },
            { "pickup", MxLogCategory.Inventory },
            { "pickups", MxLogCategory.Inventory },
            { "supply", MxLogCategory.Inventory },
            { "supplies", MxLogCategory.Inventory },
            { "client", MxLogCategory.Network },
            { "host", MxLogCategory.Network },
            { "lobby", MxLogCategory.Network },
            { "mirror", MxLogCategory.Network },
            { "network", MxLogCategory.Network },
            { "relay", MxLogCategory.Network },
            { "server", MxLogCategory.Network },
            { "session", MxLogCategory.Network },
            { "character", MxLogCategory.Player },
            { "player", MxLogCategory.Player },
            { "dialog", MxLogCategory.UI },
            { "hud", MxLogCategory.UI },
            { "menu", MxLogCategory.UI },
            { "overlay", MxLogCategory.UI },
            { "ui", MxLogCategory.UI }
        };

        private static HashSet<MxLogCategory> DisabledCategories => Config != null ? Config.DisabledCategoriesSet : _defaultDisabledCategories;
        private static Dictionary<MxLogCategory, string> Colors => Config != null ? Config.ColorsDict : _defaultColors;
        private static Dictionary<string, MxLogCategory> Keywords => Config != null ? Config.KeywordsDict : _defaultKeywords;

        #region Public API

        public static void L(string message) => LogInternal(message, null, LogType.Log);

        public static void W(string message) => LogInternal(message, null, LogType.Warning);

        public static void E(string message) => LogInternal(message, null, LogType.Error);

        public static void Ex(Exception ex) => LogException(ex, null, null);
        public static void Ex(string message, Exception ex) => LogException(ex, message, null);

        #endregion

        #region Internal

        private static void LogInternal(string message, MxLogCategory? category, LogType type)
        {
            if (!IsEnabled) return;

            (string className, string methodName) = GetCallerInfo();
            MxLogCategory cat = category ?? DetectCategory(className, methodName, message);

            if (DisabledCategories.Contains(cat)) return;

            bool hasColor = Colors.TryGetValue(cat, out string color);

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
            return (from kvp in Keywords where source.Contains(kvp.Key) select kvp.Value).FirstOrDefault();
        }

        private static void LogException(Exception ex, string additionalMessage, MxLogCategory? category)
        {
            if (!IsEnabled || ex == null) return;

            (string className, string methodName) = GetCallerInfo();
            MxLogCategory cat = category ?? DetectCategory(className, methodName, additionalMessage);

            if (DisabledCategories.Contains(cat)) return;

            bool hasColor = Colors.TryGetValue(cat, out string color);

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
