using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MxUnikit.Timer
{
    public static class MxTimer
    {
        private static int _nextId = 1;
        private static float _globalTimeScale = 1f;
        private static bool _globalPaused;

        private static readonly List<MxTimerItem> _scheduledTimers = new List<MxTimerItem>();
        private static readonly List<MxTimerItem> _conditionalTimers = new List<MxTimerItem>();
        private static readonly List<MxTimerItem> _progressTimers = new List<MxTimerItem>();
        private static readonly Dictionary<int, MxTimerItem> _timerLookup = new Dictionary<int, MxTimerItem>();
        private static readonly Dictionary<object, HashSet<int>> _ownerToTimerIds = new Dictionary<object, HashSet<int>>(); // O(1) owner lookup
        private static readonly Stack<MxTimerItem> _pool = new Stack<MxTimerItem>();
        private static readonly List<int> _toRemoveIndices = new List<int>(); // swap-and-pop removal
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

        public static int ActiveCount => _scheduledTimers.Count + _conditionalTimers.Count + _progressTimers.Count;

        #endregion

        #region Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _nextId = 1;
            _globalTimeScale = 1f;
            _globalPaused = false;
            _scheduledTimers.Clear();
            _conditionalTimers.Clear();
            _progressTimers.Clear();
            _timerLookup.Clear();
            _ownerToTimerIds.Clear();
            _pool.Clear();
            _toRemoveIndices.Clear();
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
                _conditionalTimers.Add(item);
            }

            _timerLookup[id] = item;
            RegisterOwner(id, owner);

            return new MxTimerHandle(id);
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
            item.Type = MxTimerType.Time;
            item.State = MxTimerState.Active;
            item.Duration = duration;
            item.DurationReciprocal = duration > 0f ? 1f / duration : 0f;
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
                _progressTimers.Add(item);
            }

            _timerLookup[id] = item;
            RegisterOwner(id, owner);

            return new MxTimerHandle(id);
        }

        public static MxTimerHandle RunForUnscaled(float duration, Action<float> onProgress, Action onComplete = null, object owner = null)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
            item.Type = MxTimerType.TimeUnscaled;
            item.State = MxTimerState.Active;
            item.Duration = duration;
            item.DurationReciprocal = duration > 0f ? 1f / duration : 0f;
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
                _progressTimers.Add(item);
            }

            _timerLookup[id] = item;
            RegisterOwner(id, owner);

            return new MxTimerHandle(id);
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
            // O(1) owner lookup instead of O(n) iteration
            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerIds)) return;

            foreach (int id in timerIds)
            {
                if (_timerLookup.TryGetValue(id, out MxTimerItem timer))
                {
                    timer.State = MxTimerState.Cancelled;
                }
            }
        }

        public static void PauseAll(object owner)
        {
            // O(1) owner lookup instead of O(n) iteration
            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerIds)) return;

            foreach (int id in timerIds)
            {
                if (_timerLookup.TryGetValue(id, out MxTimerItem timer) && timer.State == MxTimerState.Active)
                {
                    timer.State = MxTimerState.Paused;
                }
            }
        }

        public static void ResumeAll(object owner)
        {
            // O(1) owner lookup instead of O(n) iteration
            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerIds)) return;

            foreach (int id in timerIds)
            {
                if (_timerLookup.TryGetValue(id, out MxTimerItem timer) && timer.State == MxTimerState.Paused)
                {
                    timer.State = MxTimerState.Active;
                }
            }
        }

        public static int CountFor(object owner)
        {
            // O(1) owner lookup instead of O(n) iteration
            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerIds)) return 0;

            int count = 0;
            foreach (int id in timerIds)
            {
                if (_timerLookup.TryGetValue(id, out MxTimerItem timer) && timer.State == MxTimerState.Active)
                {
                    count++;
                }
            }
            return count;
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
            foreach (MxTimerItem scheduledTimer in _scheduledTimers)
            {
                scheduledTimer.Reset();
                _pool.Push(scheduledTimer);
            }
            foreach (MxTimerItem conditionalTimer in _conditionalTimers)
            {
                conditionalTimer.Reset();
                _pool.Push(conditionalTimer);
            }
            foreach (MxTimerItem progressTimer in _progressTimers)
            {
                progressTimer.Reset();
                _pool.Push(progressTimer);
            }

            _scheduledTimers.Clear();
            _conditionalTimers.Clear();
            _progressTimers.Clear();
            _timerLookup.Clear();
            _ownerToTimerIds.Clear();
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

            _toRemoveIndices.Clear();
            _toAdd.Clear();
            _isUpdating = true;

            // process scheduled timers (most common, hottest path)
            for (int i = 0; i < _scheduledTimers.Count; i++)
            {
                MxTimerItem timer = _scheduledTimers[i];

                // bitwise check for inactive states
                if ((timer.State & MxTimerState.Inactive) != 0)
                {
                    if (timer.State >= MxTimerState.Completed)
                    {
                        _toRemoveIndices.Add(i);
                    }
                    continue;
                }

                bool completed = false;

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

                if (completed)
                {
                    _toRemoveIndices.Add(i);
                }
            }

            // process conditional timers (polling overhead)
            for (int i = 0; i < _conditionalTimers.Count; i++)
            {
                MxTimerItem timer = _conditionalTimers[i];

                // bitwise check for inactive states
                if ((timer.State & MxTimerState.Inactive) != 0)
                {
                    if (timer.State >= MxTimerState.Completed)
                    {
                        _toRemoveIndices.Add(i | (1 << 30)); // encode list 1
                    }
                    continue;
                }

                // guaranteed to have Condition
                if (!timer.WaitUntilCondition || !timer.Condition()) continue;

                InvokeCallback(timer);
                _toRemoveIndices.Add(i | (1 << 30)); // encode list 1
            }

            // process progress timers (per-frame callbacks)
            for (int i = 0; i < _progressTimers.Count; i++)
            {
                MxTimerItem timer = _progressTimers[i];

                // bitwise check for inactive states
                if ((timer.State & MxTimerState.Inactive) != 0)
                {
                    if (timer.State >= MxTimerState.Completed)
                    {
                        _toRemoveIndices.Add(i | (2 << 30)); // encode list 2
                    }
                    continue;
                }

                float dt = timer.Type == MxTimerType.TimeUnscaled ? unscaledDeltaTime : scaledDelta;
                timer.Remaining -= dt;

                if (timer.Remaining <= 0f)
                {
                    timer.ProgressCallback(1f);
                    InvokeCallback(timer);
                    _toRemoveIndices.Add(i | (2 << 30)); // encode list 2
                }
                else
                {
                    float progress = (timer.Duration - timer.Remaining) * timer.DurationReciprocal;
                    timer.ProgressCallback(progress);
                }
            }

            _isUpdating = false;

            // swap-and-pop removal in reverse order, decode list index
            for (int i = _toRemoveIndices.Count - 1; i >= 0; i--)
            {
                int encoded = _toRemoveIndices[i];
                int listIndex = encoded >> 30;
                int idx = encoded & 0x3FFFFFFF;

                List<MxTimerItem> list = listIndex == 0 ? _scheduledTimers : (listIndex == 1 ? _conditionalTimers : _progressTimers);
                int lastIdx = list.Count - 1;

                MxTimerItem timer = list[idx];
                _timerLookup.Remove(timer.Id);
                UnregisterOwner(timer.Id, timer.Owner);
                timer.Reset();
                _pool.Push(timer);

                // swap with last element for O(1) removal
                if (idx != lastIdx)
                {
                    list[idx] = list[lastIdx];
                }
                list.RemoveAt(lastIdx);
            }

            // defer-add: distribute to appropriate lists
            if (_toAdd.Count <= 0) return;

            foreach (MxTimerItem timer in _toAdd)
            {
                if (timer.ProgressCallback != null)
                {
                    _progressTimers.Add(timer);
                }
                else if (timer.Condition != null)
                {
                    _conditionalTimers.Add(timer);
                }
                else
                {
                    _scheduledTimers.Add(timer);
                }
            }

            _toAdd.Clear();
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

        #endregion

        #region Private Helpers

        private static MxTimerHandle AddTimer(float duration, Action callback, MxTimerType type, bool repeat, int maxRepeat, object owner)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
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
                _scheduledTimers.Add(item);
            }

            _timerLookup[id] = item;
            RegisterOwner(id, owner);

            return new MxTimerHandle(id);
        }

        private static MxTimerHandle AddFrameTimer(int frameDuration, Action callback, bool repeat, int maxRepeat, object owner)
        {
            MxTimerItem item = GetPooledItem();
            int id = _nextId++;

            item.Id = id;
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
                _scheduledTimers.Add(item);
            }

            _timerLookup[id] = item;
            RegisterOwner(id, owner);

            return new MxTimerHandle(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MxTimerItem GetPooledItem()
        {
            return _pool.Count > 0 ? _pool.Pop() : new MxTimerItem();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetTimer(MxTimerHandle handle, out MxTimerItem timer)
        {
            return _timerLookup.TryGetValue(handle.Id, out timer);
        }

        // register timer with owner for O(1) lookup
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterOwner(int timerId, object owner)
        {
            if (owner == null) return;

            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerSet))
            {
                timerSet = new HashSet<int>();
                _ownerToTimerIds[owner] = timerSet;
            }
            timerSet.Add(timerId);
        }

        // unregister timer from owner
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnregisterOwner(int timerId, object owner)
        {
            if (owner == null) return;

            if (!_ownerToTimerIds.TryGetValue(owner, out HashSet<int> timerSet)) return;

            timerSet.Remove(timerId);
            // cleanup empty sets to avoid memory leak
            if (timerSet.Count == 0)
            {
                _ownerToTimerIds.Remove(owner);
            }
        }

        #endregion
    }
}
