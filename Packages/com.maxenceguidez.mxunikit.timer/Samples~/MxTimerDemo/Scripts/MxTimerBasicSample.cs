using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: Schedule, ScheduleUnscaled, ScheduleFrames, ScheduleNextFrame
    /// </summary>
    public class MxTimerBasicSample : MonoBehaviour
    {
        [Header("Schedule Settings")]
        [SerializeField] private float _scheduleDelay = 2f;
        [SerializeField] private float _unscaledDelay = 2f;
        [SerializeField] private int _frameDelay = 60;

        private MxTimerHandle _scheduleHandle;
        private MxTimerHandle _unscaledHandle;
        private MxTimerHandle _frameHandle;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 50, 300, 450));
            GUILayout.Label("<b>BASIC SCHEDULING</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Schedule (scaled time)
            GUILayout.Label($"Schedule (delay: {_scheduleDelay}s)");
            if (GUILayout.Button("Schedule"))
            {
                _scheduleHandle = MxTimer.Schedule(_scheduleDelay, () => Debug.Log($"[Schedule] Fired after {_scheduleDelay}s"), this);
                Debug.Log($"[Schedule] Started - will fire in {_scheduleDelay}s");
            }
            if (_scheduleHandle.IsValid && MxTimer.Exists(_scheduleHandle))
            {
                GUILayout.Label($"  Remaining: {MxTimer.GetRemaining(_scheduleHandle):F2}s");
            }
            GUILayout.Space(5);

            // ScheduleUnscaled
            GUILayout.Label($"ScheduleUnscaled (delay: {_unscaledDelay}s)");
            if (GUILayout.Button("ScheduleUnscaled"))
            {
                _unscaledHandle = MxTimer.ScheduleUnscaled(_unscaledDelay, () => Debug.Log($"[ScheduleUnscaled] Fired after {_unscaledDelay}s (ignores TimeScale)"), this);
                Debug.Log($"[ScheduleUnscaled] Started - will fire in {_unscaledDelay}s (ignores TimeScale)");
            }
            if (_unscaledHandle.IsValid && MxTimer.Exists(_unscaledHandle))
            {
                GUILayout.Label($"  Remaining: {MxTimer.GetRemaining(_unscaledHandle):F2}s");
            }
            GUILayout.Space(5);

            // ScheduleFrames
            GUILayout.Label($"ScheduleFrames (frames: {_frameDelay})");
            if (GUILayout.Button("ScheduleFrames"))
            {
                _frameHandle = MxTimer.ScheduleFrames(_frameDelay, () => Debug.Log($"[ScheduleFrames] Fired after {_frameDelay} frames"), this);
                Debug.Log($"[ScheduleFrames] Started - will fire in {_frameDelay} frames");
            }
            if (_frameHandle.IsValid && MxTimer.Exists(_frameHandle))
            {
                GUILayout.Label($"  Remaining: {MxTimer.GetRemaining(_frameHandle):F0} frames");
            }
            GUILayout.Space(5);

            // ScheduleNextFrame
            GUILayout.Label("ScheduleNextFrame");
            if (GUILayout.Button("ScheduleNextFrame"))
            {
                int currentFrame = Time.frameCount;
                MxTimer.ScheduleNextFrame(() => Debug.Log($"[ScheduleNextFrame] Fired on frame {Time.frameCount} (was {currentFrame})"), this);
                Debug.Log($"[ScheduleNextFrame] Started on frame {currentFrame}");
            }

            GUILayout.Space(20);
            GUILayout.Label($"Time.timeScale: {Time.timeScale:F2}");
            GUILayout.Label($"Active timers: {MxTimer.ActiveCount}");

            GUILayout.EndArea();
        }

        private void OnDestroy()
        {
            MxTimer.CancelAll(this);
        }
    }
}
