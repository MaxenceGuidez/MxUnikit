using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: GlobalTimeScale, GlobalPaused, PauseAll, ResumeAll, ClearAll, ActiveCount
    /// </summary>
    public class MxTimerGlobalSample : MonoBehaviour
    {
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(940, 50, 600, 450));
            GUILayout.BeginHorizontal();

            // Left column - Controls
            GUILayout.BeginVertical(GUILayout.Width(280));
            GUILayout.Label("<b>GLOBAL CONTROL</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Unity TimeScale
            GUILayout.Label($"Unity Time.timeScale: {Time.timeScale:F2}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0")) Time.timeScale = 0f;
            if (GUILayout.Button("0.5")) Time.timeScale = 0.5f;
            if (GUILayout.Button("1")) Time.timeScale = 1f;
            if (GUILayout.Button("2")) Time.timeScale = 2f;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // MxTimer GlobalTimeScale
            GUILayout.Label($"MxTimer.GlobalTimeScale: {MxTimer.GlobalTimeScale:F2}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0")) MxTimer.GlobalTimeScale = 0f;
            if (GUILayout.Button("0.5")) MxTimer.GlobalTimeScale = 0.5f;
            if (GUILayout.Button("1")) MxTimer.GlobalTimeScale = 1f;
            if (GUILayout.Button("2")) MxTimer.GlobalTimeScale = 2f;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Global Pause
            GUILayout.Label($"MxTimer.GlobalPaused: {MxTimer.GlobalPaused}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PauseAll()"))
            {
                MxTimer.PauseAll();
                Debug.Log("[Global] All timers paused");
            }
            if (GUILayout.Button("ResumeAll()"))
            {
                MxTimer.ResumeAll();
                Debug.Log("[Global] All timers resumed");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Clear All
            GUILayout.Label("Dangerous Operations:");
            if (GUILayout.Button("ClearAll() - Destroys all timers"))
            {
                MxTimer.ClearAll();
                Debug.Log("[Global] All timers cleared!");
            }
            GUILayout.Space(10);

            // Explanation
            GUILayout.Label("<i>Note:</i>", new GUIStyle(GUI.skin.label) { richText = true, fontStyle = FontStyle.Italic });
            GUILayout.Label("- Schedule/Repeat use Unity TimeScale");
            GUILayout.Label("  AND MxTimer.GlobalTimeScale");
            GUILayout.Label("- ScheduleUnscaled/RepeatUnscaled");
            GUILayout.Label("  ignore both time scales");

            GUILayout.EndVertical();

            GUILayout.Space(20);

            // Right column - Statistics
            GUILayout.BeginVertical(GUILayout.Width(280));
            GUILayout.Label("<b>STATISTICS</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            GUILayout.Label($"Active timers: {MxTimer.ActiveCount}");
            GUILayout.Label($"Frame: {Time.frameCount}");
            GUILayout.Label($"Time: {Time.time:F2}");
            GUILayout.Label($"Unscaled Time: {Time.unscaledTime:F2}");
            GUILayout.Space(15);

            // Effective time scale info
            float effectiveScale = Time.timeScale * MxTimer.GlobalTimeScale;
            GUILayout.Label("<b>Effective Time Scales</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Scaled timers: {effectiveScale:F2}x");
            GUILayout.Label($"Unscaled timers: 1.00x (always)");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
