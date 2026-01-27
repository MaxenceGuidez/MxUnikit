using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: Cancel, Pause, Resume, Restart, IsActive, IsPaused, Exists, GetRemaining, SetRemaining, GetProgress
    /// </summary>
    public class MxTimerControlSample : MonoBehaviour
    {
        [Header("Control Settings")]
        [SerializeField] private float _timerDuration = 10f;

        private MxTimerHandle _controlHandle;
        private int _tickCount;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(320, 500, 300, 500));
            GUILayout.Label("<b>TIMER CONTROL</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Create timer
            GUILayout.Label($"Repeating timer (interval: 1s)");
            if (GUILayout.Button("Create Timer"))
            {
                if (_controlHandle.IsValid) MxTimer.Cancel(_controlHandle);
                _tickCount = 0;
                _controlHandle = MxTimer.Repeat(1f, () =>
                {
                    _tickCount++;
                    Debug.Log($"[Control] Tick #{_tickCount}");
                }, this);
                Debug.Log("[Control] Timer created");
            }
            GUILayout.Space(5);

            // Status display
            bool exists = _controlHandle.IsValid && MxTimer.Exists(_controlHandle);
            bool isActive = _controlHandle.IsValid && MxTimer.IsActive(_controlHandle);
            bool isPaused = _controlHandle.IsValid && MxTimer.IsPaused(_controlHandle);
            float remaining = _controlHandle.IsValid ? MxTimer.GetRemaining(_controlHandle) : 0f;

            GUILayout.Label($"Exists: {exists}");
            GUILayout.Label($"IsActive: {isActive}");
            GUILayout.Label($"IsPaused: {isPaused}");
            GUILayout.Label($"Remaining: {remaining:F2}s");
            GUILayout.Label($"Tick count: {_tickCount}");
            GUILayout.Space(10);

            // Control buttons
            GUILayout.Label("Control Methods:");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause"))
            {
                bool success = MxTimer.Pause(_controlHandle);
                Debug.Log($"[Control] Pause: {(success ? "success" : "failed")}");
            }
            if (GUILayout.Button("Resume"))
            {
                bool success = MxTimer.Resume(_controlHandle);
                Debug.Log($"[Control] Resume: {(success ? "success" : "failed")}");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Restart"))
            {
                bool success = MxTimer.Restart(_controlHandle);
                Debug.Log($"[Control] Restart: {(success ? "success" : "failed")}");
            }
            if (GUILayout.Button("Cancel"))
            {
                bool success = MxTimer.Cancel(_controlHandle);
                Debug.Log($"[Control] Cancel: {(success ? "success" : "failed")}");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // SetRemaining
            GUILayout.Label("SetRemaining:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set 0.1s"))
            {
                bool success = MxTimer.SetRemaining(_controlHandle, 0.1f);
                Debug.Log($"[Control] SetRemaining(0.1): {(success ? "success" : "failed")}");
            }
            if (GUILayout.Button("Set 5s"))
            {
                bool success = MxTimer.SetRemaining(_controlHandle, 5f);
                Debug.Log($"[Control] SetRemaining(5): {(success ? "success" : "failed")}");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Progress timer for GetProgress test
            GUILayout.Label("GetProgress test:");
            if (GUILayout.Button($"Create {_timerDuration}s Timer"))
            {
                if (_controlHandle.IsValid) MxTimer.Cancel(_controlHandle);
                _controlHandle = MxTimer.Schedule(_timerDuration, () =>
                {
                    Debug.Log("[Control] Long timer complete!");
                }, this);
                Debug.Log($"[Control] Created {_timerDuration}s timer");
            }
            if (_controlHandle.IsValid && MxTimer.Exists(_controlHandle))
            {
                float progress = MxTimer.GetProgress(_controlHandle);
                GUILayout.Label($"Progress: {progress * 100:F1}%");

                Rect rect = GUILayoutUtility.GetRect(280, 15);
                GUI.Box(rect, "");
                Color oldColor = GUI.color;
                GUI.color = Color.cyan;
                GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * progress, rect.height - 4), Texture2D.whiteTexture);
                GUI.color = oldColor;
            }

            GUILayout.EndArea();
        }

        private void OnDestroy()
        {
            MxTimer.CancelAll(this);
        }
    }
}
