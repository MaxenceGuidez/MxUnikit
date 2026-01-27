using System;

namespace MxUnikit.Timer
{
    internal class MxTimerItem
    {
        public int Id;
        public MxTimerType Type;
        public MxTimerState State;

        public float Duration;
        public float DurationReciprocal; // 1 / Duration for fast progress calc
        public float Remaining;
        public int FrameDuration;
        public int FrameRemaining;

        public bool Repeat;
        public int RepeatCount;
        public int MaxRepeatCount;

        public Action Callback;
        public Action<float> ProgressCallback;
        public Func<bool> Condition;
        public bool WaitUntilCondition;

        public object Owner;

        public void Reset()
        {
            Id = 0;
            Type = MxTimerType.Time;
            State = MxTimerState.Completed;
            Duration = 0f;
            DurationReciprocal = 0f;
            Remaining = 0f;
            FrameDuration = 0;
            FrameRemaining = 0;
            Repeat = false;
            RepeatCount = 0;
            MaxRepeatCount = -1;
            Callback = null;
            ProgressCallback = null;
            Condition = null;
            WaitUntilCondition = false;
            Owner = null;
        }
    }
}
