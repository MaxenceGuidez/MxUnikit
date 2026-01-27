using System;
using System.Collections.Generic;

namespace MxUnikit.Timer
{
    public class MxTimerSequence
    {
        private readonly List<SequenceStep> _steps = new List<SequenceStep>();
        private readonly object _owner;
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
            public abstract MxTimerHandle Execute(MxTimerSequence sequence, object owner);
        }

        private class DelayStep : SequenceStep
        {
            public float Duration;
            public bool Unscaled;

            public override MxTimerHandle Execute(MxTimerSequence sequence, object owner)
            {
                return Unscaled
                    ? MxTimer.ScheduleUnscaled(Duration, sequence.Next, owner)
                    : MxTimer.Schedule(Duration, sequence.Next, owner);
            }
        }

        private class FrameDelayStep : SequenceStep
        {
            public int Frames;

            public override MxTimerHandle Execute(MxTimerSequence sequence, object owner)
            {
                return MxTimer.ScheduleFrames(Frames, sequence.Next, owner);
            }
        }

        private class CallStep : SequenceStep
        {
            public Action Callback;

            public override MxTimerHandle Execute(MxTimerSequence sequence, object owner)
            {
                Callback?.Invoke();
                sequence.Next();
                return MxTimerHandle.Invalid;
            }
        }

        private class WaitUntilStep : SequenceStep
        {
            public Func<bool> Condition;

            public override MxTimerHandle Execute(MxTimerSequence sequence, object owner)
            {
                return MxTimer.WaitUntil(Condition, sequence.Next, owner);
            }
        }

        private class RunForStep : SequenceStep
        {
            public float Duration;
            public Action<float> OnProgress;
            public bool Unscaled;

            public override MxTimerHandle Execute(MxTimerSequence sequence, object owner)
            {
                return Unscaled
                    ? MxTimer.RunForUnscaled(Duration, OnProgress, sequence.Next, owner)
                    : MxTimer.RunFor(Duration, OnProgress, sequence.Next, owner);
            }
        }

        internal MxTimerSequence(object owner)
        {
            _owner = owner;
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

            if (_currentHandle.IsValid)
                MxTimer.Cancel(_currentHandle);

            _running = false;
            _paused = false;
            _currentIndex = 0;
            _currentHandle = MxTimerHandle.Invalid;
        }

        public void Pause()
        {
            if (!_running || _paused) return;
            _paused = true;

            if (_currentHandle.IsValid)
                MxTimer.Pause(_currentHandle);
        }

        public void Resume()
        {
            if (!_running || !_paused) return;
            _paused = false;

            if (_currentHandle.IsValid)
                MxTimer.Resume(_currentHandle);
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
                    _onComplete?.Invoke();
                }
                return;
            }

            ExecuteCurrent();
        }

        private void ExecuteCurrent()
        {
            if (_currentIndex >= _steps.Count) return;
            _currentHandle = _steps[_currentIndex].Execute(this, _owner);
        }

        #endregion
    }
}
