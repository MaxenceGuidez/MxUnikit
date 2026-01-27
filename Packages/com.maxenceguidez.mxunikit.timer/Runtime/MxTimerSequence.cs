using System;
using System.Collections.Generic;

namespace MxUnikit.Timer
{
    public class MxTimerSequence
    {
        private readonly List<SequenceStep> _steps = new List<SequenceStep>();
        private readonly object _externalOwner;
        private int _currentIndex;
        private MxTimerHandle _currentHandle;
        private bool _running;
        private bool _paused;
        private bool _loop;
        private Action _onComplete;

        public bool IsRunning => _running && !_paused;
        public bool IsPaused => _running && _paused;

        private abstract class SequenceStep
        {
            public abstract MxTimerHandle Execute(MxTimerSequence sequence);
        }

        private class DelayStep : SequenceStep
        {
            public float Duration;
            public bool Unscaled;

            public override MxTimerHandle Execute(MxTimerSequence sequence)
            {
                return Unscaled
                    ? MxTimer.ScheduleUnscaled(Duration, sequence.Next, sequence)
                    : MxTimer.Schedule(Duration, sequence.Next, sequence);
            }
        }

        private class FrameDelayStep : SequenceStep
        {
            public int Frames;

            public override MxTimerHandle Execute(MxTimerSequence sequence)
            {
                return MxTimer.ScheduleFrames(Frames, sequence.Next, sequence);
            }
        }

        private class CallStep : SequenceStep
        {
            public Action Callback;

            public override MxTimerHandle Execute(MxTimerSequence sequence)
            {
                Callback?.Invoke();
                sequence.Next();
                return MxTimerHandle.Invalid;
            }
        }

        private class WaitUntilStep : SequenceStep
        {
            public Func<bool> Condition;

            public override MxTimerHandle Execute(MxTimerSequence sequence)
            {
                return MxTimer.WaitUntil(Condition, sequence.Next, sequence);
            }
        }

        private class RunForStep : SequenceStep
        {
            public float Duration;
            public Action<float> OnProgress;
            public bool Unscaled;

            public override MxTimerHandle Execute(MxTimerSequence sequence)
            {
                return Unscaled
                    ? MxTimer.RunForUnscaled(Duration, OnProgress, sequence.Next, sequence)
                    : MxTimer.RunFor(Duration, OnProgress, sequence.Next, sequence);
            }
        }

        internal MxTimerSequence(object owner)
        {
            _externalOwner = owner;
        }

        #region Builder Methods

        public MxTimerSequence Delay(float seconds)
        {
            _steps.Add(new DelayStep { Duration = seconds, Unscaled = false });
            return this;
        }

        public MxTimerSequence DelayUnscaled(float seconds)
        {
            _steps.Add(new DelayStep { Duration = seconds, Unscaled = true });
            return this;
        }

        public MxTimerSequence DelayFrames(int frames)
        {
            _steps.Add(new FrameDelayStep { Frames = frames });
            return this;
        }

        public MxTimerSequence Call(Action callback)
        {
            _steps.Add(new CallStep { Callback = callback });
            return this;
        }

        public MxTimerSequence WaitUntil(Func<bool> condition)
        {
            _steps.Add(new WaitUntilStep { Condition = condition });
            return this;
        }

        public MxTimerSequence WaitWhile(Func<bool> condition)
        {
            _steps.Add(new WaitUntilStep { Condition = () => !condition() });
            return this;
        }

        public MxTimerSequence RunFor(float duration, Action<float> onProgress)
        {
            _steps.Add(new RunForStep { Duration = duration, OnProgress = onProgress, Unscaled = false });
            return this;
        }

        public MxTimerSequence RunForUnscaled(float duration, Action<float> onProgress)
        {
            _steps.Add(new RunForStep { Duration = duration, OnProgress = onProgress, Unscaled = true });
            return this;
        }

        public MxTimerSequence Loop()
        {
            _loop = true;
            return this;
        }

        public MxTimerSequence OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        #endregion

        #region Control

        public MxTimerSequence Start()
        {
            if (_running || _steps.Count == 0) return this;

            _running = true;
            _paused = false;
            _currentIndex = 0;
            ExecuteCurrent();
            return this;
        }

        public void Stop()
        {
            if (!_running) return;

            _running = false;
            _paused = false;
            _currentIndex = 0;
            _currentHandle = MxTimerHandle.Invalid;

            MxTimer.CancelAll(this);
        }

        public void Pause()
        {
            if (!_running || _paused) return;
            _paused = true;

            MxTimer.PauseAll(this);
        }

        public void Resume()
        {
            if (!_running || !_paused) return;
            _paused = false;

            MxTimer.ResumeAll(this);
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        #endregion

        #region Internal

        private void Next()
        {
            if (!_running) return;

            _currentIndex++;

            if (_currentIndex >= _steps.Count)
            {
                if (_loop)
                {
                    _currentIndex = 0;
                    ExecuteCurrent();
                }
                else
                {
                    _running = false;
                    _currentHandle = MxTimerHandle.Invalid;
                    _onComplete?.Invoke();
                }
                return;
            }

            ExecuteCurrent();
        }

        private void ExecuteCurrent()
        {
            if (_currentIndex >= _steps.Count) return;
            _currentHandle = _steps[_currentIndex].Execute(this);
        }

        #endregion
    }
}
