using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: Repeat, RepeatUnscaled, RepeatFrames (infinite and limited count)
    /// </summary>
    public class MxTimerRepeatSample : MonoBehaviour
    {
        [Header("Repeat Settings")]
        [SerializeField] private float _repeatInterval = 1f;
        [SerializeField] private float _unscaledInterval = 1f;
        [SerializeField] private int _frameInterval = 30;
        [SerializeField] private int _repeatCount = 5;

        private MxTimerHandle _repeatHandle;
        private MxTimerHandle _repeatLimitedHandle;
        private MxTimerHandle _unscaledHandle;
        private MxTimerHandle _frameHandle;
        private MxTimerHandle _frameLimitedHandle;

        private int _repeatCounter;
        private int _repeatLimitedCounter;
        private int _unscaledCounter;
        private int _frameCounter;
        private int _frameLimitedCounter;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(320, 50, 300, 450));
            GUILayout.Label("<b>REPEAT TIMERS</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Repeat (infinite)
            GUILayout.Label($"Repeat (interval: {_repeatInterval}s) - Count: {_repeatCounter}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                if (_repeatHandle.IsValid) MxTimer.Cancel(_repeatHandle);
                _repeatCounter = 0;
                _repeatHandle = MxTimer.Repeat(_repeatInterval, () =>
                {
                    _repeatCounter++;
                    Debug.Log($"[Repeat] Tick #{_repeatCounter}");
                }, this);
                Debug.Log("[Repeat] Started infinite repeat");
            }
            if (GUILayout.Button("Stop"))
            {
                MxTimer.Cancel(_repeatHandle);
                Debug.Log("[Repeat] Stopped");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Repeat (limited count)
            GUILayout.Label($"Repeat x{_repeatCount} (interval: {_repeatInterval}s) - Count: {_repeatLimitedCounter}");
            if (GUILayout.Button($"Start ({_repeatCount} times)"))
            {
                if (_repeatLimitedHandle.IsValid) MxTimer.Cancel(_repeatLimitedHandle);
                _repeatLimitedCounter = 0;
                _repeatLimitedHandle = MxTimer.Repeat(_repeatInterval, _repeatCount, () =>
                {
                    _repeatLimitedCounter++;
                    Debug.Log($"[RepeatLimited] Tick #{_repeatLimitedCounter}/{_repeatCount}");
                }, this);
                Debug.Log($"[RepeatLimited] Started - will repeat {_repeatCount} times");
            }
            GUILayout.Space(5);

            // RepeatUnscaled
            GUILayout.Label($"RepeatUnscaled (interval: {_unscaledInterval}s) - Count: {_unscaledCounter}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                if (_unscaledHandle.IsValid) MxTimer.Cancel(_unscaledHandle);
                _unscaledCounter = 0;
                _unscaledHandle = MxTimer.RepeatUnscaled(_unscaledInterval, () =>
                {
                    _unscaledCounter++;
                    Debug.Log($"[RepeatUnscaled] Tick #{_unscaledCounter} (ignores TimeScale)");
                }, this);
                Debug.Log("[RepeatUnscaled] Started (ignores TimeScale)");
            }
            if (GUILayout.Button("Stop"))
            {
                MxTimer.Cancel(_unscaledHandle);
                Debug.Log("[RepeatUnscaled] Stopped");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // RepeatFrames (infinite)
            GUILayout.Label($"RepeatFrames (interval: {_frameInterval} frames) - Count: {_frameCounter}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                if (_frameHandle.IsValid) MxTimer.Cancel(_frameHandle);
                _frameCounter = 0;
                _frameHandle = MxTimer.RepeatFrames(_frameInterval, () =>
                {
                    _frameCounter++;
                    Debug.Log($"[RepeatFrames] Tick #{_frameCounter} at frame {Time.frameCount}");
                }, this);
                Debug.Log("[RepeatFrames] Started");
            }
            if (GUILayout.Button("Stop"))
            {
                MxTimer.Cancel(_frameHandle);
                Debug.Log("[RepeatFrames] Stopped");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // RepeatFrames (limited count)
            GUILayout.Label($"RepeatFrames x{_repeatCount} (interval: {_frameInterval}) - Count: {_frameLimitedCounter}");
            if (GUILayout.Button($"Start ({_repeatCount} times)"))
            {
                if (_frameLimitedHandle.IsValid) MxTimer.Cancel(_frameLimitedHandle);
                _frameLimitedCounter = 0;
                _frameLimitedHandle = MxTimer.RepeatFrames(_frameInterval, _repeatCount, () =>
                {
                    _frameLimitedCounter++;
                    Debug.Log($"[RepeatFramesLimited] Tick #{_frameLimitedCounter}/{_repeatCount}");
                }, this);
                Debug.Log($"[RepeatFramesLimited] Started - will repeat {_repeatCount} times");
            }

            GUILayout.Space(20);
            GUILayout.Label($"Active timers: {MxTimer.ActiveCount}");

            GUILayout.EndArea();
        }

        private void OnDestroy()
        {
            MxTimer.CancelAll(this);
        }
    }
}
