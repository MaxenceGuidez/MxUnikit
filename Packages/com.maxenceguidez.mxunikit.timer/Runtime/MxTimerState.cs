namespace MxUnikit.Timer
{
    [System.Flags]
    internal enum MxTimerState : byte
    {
        Active = 0,
        Paused = 1,
        Completed = 2,
        Cancelled = 4,

        // bitwise mask for inactive states
        Inactive = Paused | Completed | Cancelled
    }
}
