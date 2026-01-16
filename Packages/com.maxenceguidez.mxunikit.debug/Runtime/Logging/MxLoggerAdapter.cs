using MxUnikit.Core.Logging;

namespace MxUnikit.Debug.Logging
{
    public class MxLoggerAdapter : IMxLogger
    {
        // ----------------------------------------------------------------------------------------
        public void Log(string message)
        {
            MxLogger.L(message);
        }

        // ----------------------------------------------------------------------------------------
        public void LogWarning(string message)
        {
            MxLogger.W(message);
        }

        // ----------------------------------------------------------------------------------------
        public void LogError(string message)
        {
            MxLogger.E(message);
        }

        // ----------------------------------------------------------------------------------------
    }
}