using System;

namespace MxUnikit.Debug
{
    public static class MxDebug
    {
        public static event Action<bool> OnDebugModeChanged;

        public static bool IsDebugMode { get; private set; }

        public static void SetDebugMode(bool isActive)
        {
            if (IsDebugMode == isActive) return;

            IsDebugMode = isActive;
            OnDebugModeChanged?.Invoke(IsDebugMode);
        }

        public static void Enable()
        {
            SetDebugMode(true);
        }
        public static void Disable()
        {
            SetDebugMode(false);
        }

        public static void Toggle()
        {
            SetDebugMode(!IsDebugMode);
        }
    }
}
