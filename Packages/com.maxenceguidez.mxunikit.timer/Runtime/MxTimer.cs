using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MxUnikit.Timer
{
    public static class MxTimer
    {
        private static int _nextId = 1;
        private static float _globalTimeScale = 1f;
        private static bool _globalPaused;

        private static readonly List<MxTimerItem> _activeTimers = new List<MxTimerItem>();
        private static readonly Dictionary<int, MxTimerItem> _timerLookup = new Dictionary<int, MxTimerItem>();
        private static readonly Stack<MxTimerItem> _pool = new Stack<MxTimerItem>();
        private static readonly List<MxTimerItem> _toRemove = new List<MxTimerItem>();
        private static readonly List<MxTimerItem> _toAdd = new List<MxTimerItem>();
        private static bool _isUpdating;

        private const int InitialPoolSize = 32;

        #region Properties

        public static float GlobalTimeScale
        {
            get => _globalTimeScale;
            set => _globalTimeScale = Mathf.Max(0f, value);
        }

        public static bool GlobalPaused
        {
            get => _globalPaused;
            set => _globalPaused = value;
        }

        public static int ActiveCount => _activeTimers.Count;

        #endregion

        #region Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _nextId = 1;
            _globalTimeScale = 1f;
            _globalPaused = false;
            _activeTimers.Clear();
            _timerLookup.Clear();
            _pool.Clear();
            _toRemove.Clear();
            _toAdd.Clear();
            _isUpdating = false;

            for (int i = 0; i < InitialPoolSize; i++)
            {
                _pool.Push(new MxTimerItem());
            }

            GameObject go = new GameObject("[MxTimer]") { hideFlags = HideFlags.HideAndDontSave };
            go.AddComponent<MxTimerRunner>();

            Object.DontDestroyOnLoad(go);
        }

        #endregion

        #region Schedule (Time-based)

        public static MxTimerHandle Schedule(float delay, Action callback, object owner = null)
        {
            return AddTimer(delay, callback, MxTimerType.Time, false, -1, owner);
        }

        public static MxTimerHandle ScheduleUnscaled(float delay, Action callback, object owner = null)
        {
            return AddTimer(delay, callback, MxTimerType.TimeUnscaled, false, -1, owner);
        }

        #endregion

        #region Schedule (Frame-based)

        public static MxTimerHandle ScheduleFrames(int frameDelay, Action callback, object owner = null)
        {
            return AddFrameTimer(frameDelay, callback, false, -1, owner);
        }

        public static MxTimerHandle ScheduleNextFrame(Action callback, object owner = null)
        {
            return ScheduleFrames(1, callback, owner);
        }

        #endregion

        #region Repeat

        public static MxTimerHandle Repeat(float interval, Action callback, object owner = null)
        {
            return AddTimer(interval, callback, MxTimerType.Time, true, -1, owner);
        }

        public static MxTimerHandle Repeat(float interval, int count, Action callback, object owner = null)
        {
            return AddTimer(interval, callback, MxTimerType.Time, true, count, owner);
        }

        public static MxTimerHandle RepeatUnscaled(float interval, Action callback, object owner = null)
        {
            return AddTimer(interval, callback, MxTimerType.TimeUnscaled, true, -1, owner);
        }

        public static MxTimerHandle RepeatUnscaled(float interval, int count, Action callback, object owner = null)
        {
            return AddTimer(interval, callback, MxTimerType.TimeUnscaled, true, count, owner);
        }

        public static MxTimerHandle RepeatFrames(int frameInterval, Action callback, object owner = null)
        {
            return AddFrameTimer(frameInterval, callback, true, -1, owner);
        }

        public static MxTimerHandle RepeatFrames(int frameInterval, int count, Action callback, object owner = null)
        {
            return AddFrameTimer(frameInterval, callback, true, count, owner);
        }

        #endregion

        #region Conditional Timers

        public static MxTimerHandle WaitUntil(Func<bool> condition, Action callback, object owner = null)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Version = 1;
            item.Type = MxTimerType.Frame;
            item.State = MxTimerState.Active;
            item.FrameDuration = 1;
            item.FrameRemaining = 1;
            item.Repeat = true;
            item.MaxRepeatCount = -1;
            item.Callback = callback;
            item.Condition = condition;
            item.WaitUntilCondition = true;
            item.Owner = owner;

            if (_isUpdating)
            {
                _toAdd.Add(item);
            }
            else
            {
                _activeTimers.Add(item);
            }

            _timerLookup[id] = item;

            return new MxTimerHandle(id, item.Version);
        }

        public static MxTimerHandle WaitWhile(Func<bool> condition, Action callback, object owner = null)
        {
            return WaitUntil(() => !condition(), callback, owner);
        }

        #endregion

        #region RunFor (Progress Timer)

        public static MxTimerHandle RunFor(float duration, Action<float> onProgress, Action onComplete = null, object owner = null)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Version = 1;
            item.Type = MxTimerType.Time;
            item.State = MxTimerState.Active;
            item.Duration = duration;
            item.Remaining = duration;
            item.Repeat = false;
            item.Callback = onComplete;
            item.ProgressCallback = onProgress;
            item.Owner = owner;

            if (_isUpdating)
            {
                _toAdd.Add(item);
            }
            else
            {
                _activeTimers.Add(item);
            }

            _timerLookup[id] = item;

            return new MxTimerHandle(id, item.Version);
        }

        public static MxTimerHandle RunForUnscaled(float duration, Action<float> onProgress, Action onComplete = null, object owner = null)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Version = 1;
            item.Type = MxTimerType.TimeUnscaled;
            item.State = MxTimerState.Active;
            item.Duration = duration;
            item.Remaining = duration;
            item.Repeat = false;
            item.Callback = onComplete;
            item.ProgressCallback = onProgress;
            item.Owner = owner;

            if (_isUpdating)
            {
                _toAdd.Add(item);
            }
            else
            {
                _activeTimers.Add(item);
            }

            _timerLookup[id] = item;

            return new MxTimerHandle(id, item.Version);
        }

        #endregion

        #region Control Methods

        public static bool Cancel(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return false;

            timer.State = MxTimerState.Cancelled;

            return true;
        }

        public static bool Pause(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return false;
            if (timer.State != MxTimerState.Active) return false;

            timer.State = MxTimerState.Paused;

            return true;
        }

        public static bool Resume(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return false;
            if (timer.State != MxTimerState.Paused) return false;

            timer.State = MxTimerState.Active;

            return true;
        }

        public static bool Restart(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return false;

            timer.State = MxTimerState.Active;
            timer.Remaining = timer.Duration;
            timer.FrameRemaining = timer.FrameDuration;
            timer.RepeatCount = 0;

            return true;
        }

        public static bool IsActive(MxTimerHandle handle)
        {
            return TryGetTimer(handle, out MxTimerItem timer) && timer.State == MxTimerState.Active;
        }

        public static bool IsPaused(MxTimerHandle handle)
        {
            return TryGetTimer(handle, out MxTimerItem timer) && timer.State == MxTimerState.Paused;
        }

        public static bool Exists(MxTimerHandle handle)
        {
            return TryGetTimer(handle, out _);
        }

        public static float GetRemaining(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return 0f;

            return timer.Type == MxTimerType.Frame ? timer.FrameRemaining : timer.Remaining;
        }

        public static bool SetRemaining(MxTimerHandle handle, float time)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return false;

            timer.Remaining = Mathf.Max(0f, time);

            return true;
        }

        public static float GetProgress(MxTimerHandle handle)
        {
            if (!TryGetTimer(handle, out MxTimerItem timer)) return 0f;
            if (timer.Duration <= 0f) return 1f;

            return 1f - timer.Remaining / timer.Duration;
        }

        #endregion

        #region Owner-based Control

        public static void CancelAll(object owner)
        {
            foreach (MxTimerItem timer in _activeTimers.Where(timer => timer.Owner == owner))
            {
                timer.State = MxTimerState.Cancelled;
            }
        }

        public static void PauseAll(object owner)
        {
            foreach (MxTimerItem timer in _activeTimers.Where(timer => timer.Owner == owner && timer.State == MxTimerState.Active))
            {
                timer.State = MxTimerState.Paused;
            }
        }

        public static void ResumeAll(object owner)
        {
            foreach (MxTimerItem timer in _activeTimers.Where(timer => timer.Owner == owner && timer.State == MxTimerState.Paused))
            {
                timer.State = MxTimerState.Active;
            }
        }

        public static int CountFor(object owner)
        {
            return _activeTimers.Count(timer => timer.Owner == owner && timer.State == MxTimerState.Active);
        }

        #endregion

        #region Global Control

        public static void PauseAll()
        {
            _globalPaused = true;
        }

        public static void ResumeAll()
        {
            _globalPaused = false;
        }

        public static void ClearAll()
        {
            foreach (MxTimerItem timer in _activeTimers)
            {
                timer.Reset();
                _pool.Push(timer);
            }
            _activeTimers.Clear();
            _timerLookup.Clear();
        }

        #endregion

        #region Sequence Builder

        public static MxTimerSequence Sequence()
        {
            return new MxTimerSequence();
        }

        #endregion

        #region Internal Update

        internal static void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (_globalPaused) return;

            float scaledDelta = deltaTime * _globalTimeScale;

            _toRemove.Clear();
            _toAdd.Clear();
            _isUpdating = true;

            foreach (MxTimerItem timer in _activeTimers)
            {
                switch (timer.State)
                {
                    case MxTimerState.Cancelled or MxTimerState.Completed:
                        _toRemove.Add(timer);
                        continue;
                    case MxTimerState.Paused:
                        continue;
                }

                bool completed = false;

                if (timer.Condition != null)
                {
                    bool conditionMet = timer.Condition();
                    if (timer.WaitUntilCondition && conditionMet)
                    {
                        InvokeCallback(timer);
                        completed = true;
                    }
                }
                else if (timer.ProgressCallback != null)
                {
                    float dt = timer.Type == MxTimerType.TimeUnscaled ? unscaledDeltaTime : scaledDelta;
                    timer.Remaining -= dt;

                    float progress = timer.Duration > 0f ? 1f - (timer.Remaining / timer.Duration) : 1f;
                    progress = Mathf.Clamp01(progress);

                    try
                    {
                        timer.ProgressCallback(progress);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    if (timer.Remaining <= 0f)
                    {
                        InvokeCallback(timer);
                        completed = true;
                    }
                }
                else
                {
                    switch (timer.Type)
                    {
                        case MxTimerType.Time:
                            timer.Remaining -= scaledDelta;
                            if (timer.Remaining <= 0f)
                            {
                                completed = ProcessTimerComplete(timer);
                            }
                            break;

                        case MxTimerType.TimeUnscaled:
                            timer.Remaining -= unscaledDeltaTime;
                            if (timer.Remaining <= 0f)
                            {
                                completed = ProcessTimerComplete(timer);
                            }
                            break;

                        case MxTimerType.Frame:
                            timer.FrameRemaining--;
                            if (timer.FrameRemaining <= 0)
                            {
                                completed = ProcessTimerComplete(timer);
                            }
                            break;
                    }
                }

                if (completed)
                {
                    _toRemove.Add(timer);
                }
            }

            _isUpdating = false;

            foreach (MxTimerItem timer in _toRemove)
            {
                _activeTimers.Remove(timer);
                _timerLookup.Remove(timer.Id);
                timer.Reset();
                _pool.Push(timer);
            }

            if (_toAdd.Count > 0)
            {
                _activeTimers.AddRange(_toAdd);
                _toAdd.Clear();
            }
        }

        private static bool ProcessTimerComplete(MxTimerItem timer)
        {
            InvokeCallback(timer);

            if (!timer.Repeat) return true;

            timer.RepeatCount++;

            if (timer.MaxRepeatCount > 0 && timer.RepeatCount >= timer.MaxRepeatCount) return true;

            timer.Remaining = timer.Duration + timer.Remaining;
            timer.FrameRemaining = timer.FrameDuration;

            return false;
        }

        private static void InvokeCallback(MxTimerItem timer)
        {
            try
            {
                timer.Callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Private Helpers

        private static MxTimerHandle AddTimer(float duration, Action callback, MxTimerType type, bool repeat, int maxRepeat, object owner)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Version = 1;
            item.Type = type;
            item.State = MxTimerState.Active;
            item.Duration = duration;
            item.Remaining = duration;
            item.Repeat = repeat;
            item.MaxRepeatCount = maxRepeat;
            item.Callback = callback;
            item.Owner = owner;

            if (_isUpdating)
            {
                _toAdd.Add(item);
            }
            else
            {
                _activeTimers.Add(item);
            }

            _timerLookup[id] = item;

            return new MxTimerHandle(id, item.Version);
        }

        private static MxTimerHandle AddFrameTimer(int frameDuration, Action callback, bool repeat, int maxRepeat, object owner)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Version = 1;
            item.Type = MxTimerType.Frame;
            item.State = MxTimerState.Active;
            item.FrameDuration = frameDuration;
            item.FrameRemaining = frameDuration;
            item.Repeat = repeat;
            item.MaxRepeatCount = maxRepeat;
            item.Callback = callback;
            item.Owner = owner;

            if (_isUpdating)
            {
                _toAdd.Add(item);
            }
            else
            {
                _activeTimers.Add(item);
            }

            _timerLookup[id] = item;

            return new MxTimerHandle(id, item.Version);
        }

        private static MxTimerItem GetPooledItem()
        {
            return _pool.Count > 0 ? _pool.Pop() : new MxTimerItem();
        }

        private static bool TryGetTimer(MxTimerHandle handle, out MxTimerItem timer)
        {
            if (_timerLookup.TryGetValue(handle.Id, out timer))
            {
                if (timer.Version == handle.Version) return true;
            }
            timer = null;
            return false;
        }

        #endregion
    }
}
