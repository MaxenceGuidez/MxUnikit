using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: MxTimerSequence with all builder methods
    /// (Delay, DelayUnscaled, DelayFrames, Call, WaitUntil, WaitWhile, RunFor, Loop, OnComplete)
    /// </summary>
    public class MxTimerSequenceSample : MonoBehaviour
    {
        [Header("Sequence Settings")]
        [SerializeField] private bool _waitCondition;

        private MxTimerSequence _sequence1;
        private MxTimerSequence _sequence2;
        private MxTimerSequence _loopSequence;

        private string _sequence1Status = "Not started";
        private string _sequence2Status = "Not started";
        private string _loopStatus = "Not started";

        private float _progressValue;
        private int _loopCount;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(940, 500, 350, 500));
            GUILayout.Label("<b>SEQUENCES</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Sequence 1: Basic delays and calls
            GUILayout.Label("<b>Sequence 1: Delays + Calls</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Status: {_sequence1Status}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                _sequence1?.Stop();
                _sequence1Status = "Running...";
                _sequence1 = MxTimer.Sequence()
                    .Call(() =>
                    {
                        _sequence1Status = "Step 1: Started";
                        Debug.Log("[Seq1] Step 1: Started");
                    })
                    .Delay(1f)
                    .Call(() =>
                    {
                        _sequence1Status = "Step 2: After 1s delay";
                        Debug.Log("[Seq1] Step 2: After 1s delay");
                    })
                    .DelayFrames(60)
                    .Call(() =>
                    {
                        _sequence1Status = "Step 3: After 60 frames";
                        Debug.Log("[Seq1] Step 3: After 60 frames");
                    })
                    .Delay(0.5f)
                    .Call(() =>
                    {
                        _sequence1Status = "Step 4: Final";
                        Debug.Log("[Seq1] Step 4: Final");
                    })
                    .OnComplete(() =>
                    {
                        _sequence1Status = "Complete!";
                        Debug.Log("[Seq1] Sequence complete!");
                    })
                    .Start();
            }
            if (GUILayout.Button("Pause")) _sequence1?.Pause();
            if (GUILayout.Button("Resume")) _sequence1?.Resume();
            if (GUILayout.Button("Stop")) { _sequence1?.Stop(); _sequence1Status = "Stopped"; }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Sequence 2: With conditions and progress
            GUILayout.Label("<b>Sequence 2: Conditions + RunFor</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Status: {_sequence2Status}");
            GUILayout.Label($"Progress: {_progressValue * 100:F0}%");
            DrawProgressBar(_progressValue, 300);

            GUILayout.Label($"Wait condition: {_waitCondition}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle")) _waitCondition = !_waitCondition;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                _sequence2?.Stop();
                _progressValue = 0f;
                _sequence2Status = "Running...";
                _sequence2 = MxTimer.Sequence()
                    .Call(() =>
                    {
                        _sequence2Status = "Waiting for condition...";
                        Debug.Log("[Seq2] Waiting for condition to be true...");
                    })
                    .WaitUntil(() => _waitCondition)
                    .Call(() =>
                    {
                        _sequence2Status = "Condition met! Running progress...";
                        Debug.Log("[Seq2] Condition met!");
                    })
                    .RunFor(2f, p => _progressValue = p)
                    .Call(() =>
                    {
                        _sequence2Status = "Progress complete!";
                        Debug.Log("[Seq2] Progress timer complete");
                    })
                    .OnComplete(() =>
                    {
                        _sequence2Status = "Sequence complete!";
                        Debug.Log("[Seq2] Sequence complete!");
                    })
                    .Start();
            }
            if (GUILayout.Button("Stop")) { _sequence2?.Stop(); _sequence2Status = "Stopped"; }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Loop sequence
            GUILayout.Label("<b>Sequence 3: Looping</b>", new GUIStyle(GUI.skin.label) { richText = true });
            GUILayout.Label($"Status: {_loopStatus}");
            GUILayout.Label($"Loop count: {_loopCount}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                _loopSequence?.Stop();
                _loopCount = 0;
                _loopStatus = "Looping...";
                _loopSequence = MxTimer.Sequence()
                    .Call(() =>
                    {
                        _loopCount++;
                        _loopStatus = $"Loop iteration {_loopCount}";
                        Debug.Log($"[SeqLoop] Iteration {_loopCount}");
                    })
                    .Delay(1f)
                    .Loop()
                    .Start();
            }
            if (GUILayout.Button("Pause")) _loopSequence?.Pause();
            if (GUILayout.Button("Resume")) _loopSequence?.Resume();
            if (GUILayout.Button("Stop"))
            {
                _loopSequence?.Stop();
                _loopStatus = "Stopped";
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (_sequence1 != null)
                GUILayout.Label($"Seq1 - Running: {_sequence1.IsRunning}, Paused: {_sequence1.IsPaused}");
            if (_sequence2 != null)
                GUILayout.Label($"Seq2 - Running: {_sequence2.IsRunning}, Paused: {_sequence2.IsPaused}");
            if (_loopSequence != null)
                GUILayout.Label($"SeqLoop - Running: {_loopSequence.IsRunning}, Paused: {_loopSequence.IsPaused}");

            GUILayout.EndArea();
        }

        private void DrawProgressBar(float progress, float width)
        {
            Rect rect = GUILayoutUtility.GetRect(width, 15);
            GUI.Box(rect, "");
            Color oldColor = GUI.color;
            GUI.color = Color.magenta;
            GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * progress, rect.height - 4), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private void OnDestroy()
        {
            _sequence1?.Stop();
            _sequence2?.Stop();
            _loopSequence?.Stop();
            MxTimer.CancelAll(this);
        }
    }
}
