namespace MxUnikit.Core.Logging
{
    public static class MxLogManager
    {
        // ----------------------------------------------------------------------------------------
        public static IMxLogger Logger { get; set; } = new MxUnityLogger();

        // ----------------------------------------------------------------------------------------
    }
}