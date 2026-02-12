using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MxUnikit.Log
{
    public static class MxLog
    {
        private static MxLogConfig Config => MxLogConfig.Instance;

        private static bool IsEnabled => Config == null || Config.IsEnabled;
        private static bool LogStackTraceForExceptions => Config == null || Config.LogStackTraceForExceptions;

        [ThreadStatic] private static StringBuilder _stringBuilder;

        #region Public API

        public static void L(string message) => LogInternal(message, null, null, LogType.Log);

        public static void W(string message) => LogInternal(message, null, null, LogType.Warning);

        public static void E(string message) => LogInternal(message, null, null, LogType.Error);

        public static void Ex(Exception ex) => LogInternal(null, null, ex, LogType.Exception);
        public static void Ex(string message, Exception ex) => LogInternal(message, null, ex, LogType.Exception);

        #endregion

        private static void LogInternal(string message, MxLogCategory category, Exception ex, LogType type)
        {
            if (!IsEnabled) return;
            if (ex == null && string.IsNullOrEmpty(message)) return;

            StackTrace stackTrace = new StackTrace(2, true);
            StackFrame callerFrame = stackTrace.GetFrame(0);

            if (callerFrame == null) return;

            MethodBase method = callerFrame.GetMethod();
            if (method == null) return;

            ExtractMethodInfo(method, out string className, out string methodName);

            MxLogCategory cat = category ?? DetectCategory(className, methodName, message);

            if (Config != null && !Config.IsCategoryEnabled(cat)) return;

            string color = Config?.GetCategoryColor(cat);
            bool hasColor = !string.IsNullOrEmpty(color);

            StringBuilder sb = GetStringBuilder();

            if (hasColor)
            {
                sb.Append("[<color=").Append(color).Append('>').Append(className)
                  .Append("</color>] - <color=").Append(color).Append('>')
                  .Append(methodName).Append("()</color>");
            }
            else
            {
                sb.Append('[').Append(className).Append("] - ").Append(methodName).Append("()");
            }

            if (ex != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    sb.Append(" : ").Append(message).Append('\n');
                }
                else
                {
                    sb.Append('\n');
                }

                sb.Append("<color=#FF0000>").Append(ex.GetType().Name)
                  .Append("</color>: ").Append(ex.Message);

                if (LogStackTraceForExceptions && !string.IsNullOrEmpty(ex.StackTrace))
                {
                    sb.Append('\n').Append(ex.StackTrace);
                }

                if (ex.InnerException != null)
                {
                    sb.Append("\n---> Inner Exception: <color=#FF4500>")
                      .Append(ex.InnerException.GetType().Name)
                      .Append("</color>: ").Append(ex.InnerException.Message);

                    if (LogStackTraceForExceptions && !string.IsNullOrEmpty(ex.InnerException.StackTrace))
                    {
                        sb.Append('\n').Append(ex.InnerException.StackTrace);
                    }
                }
            }
            else
            {
                sb.Append(" : ").Append(message);
            }

            sb.Append("\n\n");
            BuildCustomStackTrace(sb, stackTrace);

            Debug.LogFormat(type, LogOption.NoStacktrace, null, sb.ToString());
        }

        #region Utils

        private static void ExtractMethodInfo(MethodBase method, out string className, out string methodName)
        {
            className = method.DeclaringType?.Name ?? "Unknown";
            methodName = method.Name;

            if (!methodName.StartsWith("<")) return;

            int endIndex = methodName.IndexOf('>');
            if (endIndex > 1)
            {
                methodName = methodName[1..endIndex];
            }
        }

        private static void BuildCustomStackTrace(StringBuilder sb, StackTrace stackTrace)
        {
            int initialLength = sb.Length;

            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                string fileName = frame.GetFileName();
                if (string.IsNullOrEmpty(fileName)) continue;

                int lineNumber = frame.GetFileLineNumber();

                sb.Append(method.DeclaringType?.FullName ?? "Unknown")
                  .Append(':').Append(method.Name)
                  .Append("\t\t(<a href=\"");

                int fileNameStart = sb.Length;
                sb.Append(fileName);

                for (int j = fileNameStart; j < sb.Length; j++)
                {
                    if (sb[j] == '\\') sb[j] = '/';
                }

                sb.Append("\" line=\"").Append(lineNumber).Append("\">")
                  .Append(fileName, 0, fileName.Length);

                for (int j = sb.Length - fileName.Length; j < sb.Length; j++)
                {
                    if (sb[j] == '\\') sb[j] = '/';
                }

                sb.Append(':').Append(lineNumber).Append("</a>)\n");
            }

            if (sb.Length > initialLength && sb[^1] == '\n')
            {
                sb.Length--;
            }
        }

        private static MxLogCategory DetectCategory(string className, string methodName, string message)
        {
            if (Config == null || string.IsNullOrEmpty(className) && string.IsNullOrEmpty(methodName) &&
                string.IsNullOrEmpty(message))
            {
                return null;
            }

            MxLogCategory result = DetectCategory(className);
            if (result != null) return result;

            result = DetectCategory(methodName);
            if (result != null) return result;

            result = DetectCategory(message);
            return result;
        }

        private static MxLogCategory DetectCategory(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            int wordStart = 0;
            int length = text.Length;

            for (int i = 0; i <= length; i++)
            {
                bool isDelimiter = i == length || text[i] == ' ' || text[i] == '_' || text[i] == '-';

                if (!isDelimiter || i <= wordStart) continue;

                int wordLength = i - wordStart;
                if (wordLength > 0)
                {
                    MxLogCategory category = Config.DetectCategoryFromKeywordSegment(text, wordStart, wordLength);
                    if (category != null)
                    {
                        return category;
                    }
                }
                wordStart = i + 1;
            }

            return null;
        }

        private static StringBuilder GetStringBuilder()
        {
            if (_stringBuilder == null)
            {
                _stringBuilder = new StringBuilder(512);
            }
            else
            {
                _stringBuilder.Clear();
            }

            return _stringBuilder;
        }

        #endregion
    }
}
