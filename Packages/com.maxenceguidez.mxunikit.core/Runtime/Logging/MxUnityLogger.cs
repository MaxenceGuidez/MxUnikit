using UnityEngine;

namespace MxUnikit.Core.Logging
{
    public class MxUnityLogger : IMxLogger
    {
        // ----------------------------------------------------------------------------------------
        public void Log(string message)
        {
            Debug.Log(message);
        }

        // ----------------------------------------------------------------------------------------
        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        // ----------------------------------------------------------------------------------------
        public void LogError(string message)
        {
            Debug.LogError(message);
        }

        // ----------------------------------------------------------------------------------------
    }
}