using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: RunFor, RunForUnscaled with visual progress bars
    /// </summary>
    public class MxTimerProgressSample : MonoBehaviour
    {
        [Header("Progress Settings")]
        [SerializeField] private float _duration = 3f;

        private MxTimerHandle _runForHandle;
        private MxTimerHandle _runForUnscaledHandle;

        private float _progress1;
        private float _progress2;
        private bool _isRunning1;
        private bool _isRunning2;

        private GUIStyle _progressBarStyle;
        private GUIStyle _progressFillStyle;

        private void OnGUI()
        {
            InitStyles();

            GUILayout.BeginArea(new Rect(10, 500, 300, 500));
            GUILayout.Label("<b>PROGRESS TIMERS (RunFor)</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // RunFor (scaled)
            GUILayout.Label($"RunFor (duration: {_duration}s) - Respects TimeScale");
            DrawProgressBar(_progress1);
            GUILayout.Label($"Progress: {_progress1 * 100:F1}%");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                if (_runForHandle.IsValid) MxTimer.Cancel(_runForHandle);
                _progress1 = 0f;
                _isRunning1 = true;
                _runForHandle = MxTimer.RunFor(
                    _duration,
                    progress =>
                    {
                        _progress1 = progress;
                    },
                    () =>
                    {
                        _progress1 = 1f;
                        _isRunning1 = false;
                        Debug.Log("[RunFor] Complete!");
                    },
                    this
                );
                Debug.Log($"[RunFor] Started - {_duration}s duration");
            }
            if (GUILayout.Button("Cancel"))
            {
                MxTimer.Cancel(_runForHandle);
                _isRunning1 = false;
                Debug.Log("[RunFor] Cancelled");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label($"Running: {_isRunning1}");
            GUILayout.Space(15);

            // RunForUnscaled
            GUILayout.Label($"RunForUnscaled (duration: {_duration}s) - Ignores TimeScale");
            DrawProgressBar(_progress2);
            GUILayout.Label($"Progress: {_progress2 * 100:F1}%");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                if (_runForUnscaledHandle.IsValid) MxTimer.Cancel(_runForUnscaledHandle);
                _progress2 = 0f;
                _isRunning2 = true;
                _runForUnscaledHandle = MxTimer.RunForUnscaled(
                    _duration,
                    progress =>
                    {
                        _progress2 = progress;
                    },
                    () =>
                    {
                        _progress2 = 1f;
                        _isRunning2 = false;
                        Debug.Log("[RunForUnscaled] Complete!");
                    },
                    this
                );
                Debug.Log($"[RunForUnscaled] Started - {_duration}s duration (ignores TimeScale)");
            }
            if (GUILayout.Button("Cancel"))
            {
                MxTimer.Cancel(_runForUnscaledHandle);
                _isRunning2 = false;
                Debug.Log("[RunForUnscaled] Cancelled");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label($"Running: {_isRunning2}");

            GUILayout.Space(15);
            GUILayout.Label("<i>Tip: Change TimeScale in Global sample to see the difference</i>", new GUIStyle(GUI.skin.label) { richText = true, fontStyle = FontStyle.Italic });

            GUILayout.EndArea();
        }

        private void InitStyles()
        {
            if (_progressBarStyle == null)
            {
                _progressBarStyle = new GUIStyle(GUI.skin.box);
                _progressFillStyle = new GUIStyle();
                _progressFillStyle.normal.background = Texture2D.whiteTexture;
            }
        }

        private void DrawProgressBar(float progress)
        {
            Rect rect = GUILayoutUtility.GetRect(280, 20);
            GUI.Box(rect, "", _progressBarStyle);

            Color oldColor = GUI.color;
            GUI.color = Color.green;
            Rect fillRect = new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * progress, rect.height - 4);
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private void OnDestroy()
        {
            MxTimer.CancelAll(this);
        }
    }
}
