using MxUnikit.Core.Logging;
using MxUnikit.Debug.Logging;

namespace DefaultNamespace
{
    public static class MxDebugBootstrap
    {
        // ----------------------------------------------------------------------------------------
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallLogger()
        {
            MxLogManager.Logger = new MxLoggerAdapter();
        }

        // ----------------------------------------------------------------------------------------
    }
}