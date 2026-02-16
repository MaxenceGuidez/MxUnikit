using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: WaitUntil, WaitWhile
    /// </summary>
    public class MxTimerConditionalSample : MonoBehaviour
    {
        [Header("Conditional Settings")]
        [SerializeField] private bool _conditionFlag;
        [SerializeField] private int _targetValue = 10;

        private MxTimerHandle _waitUntilHandle;
        private MxTimerHandle _waitWhileHandle;
        private MxTimerHandle _waitUntilValueHandle;

        private int _currentValue;
        private bool _isWaitingUntil;
        private bool _isWaitingWhile;
        private bool _isWaitingUntilValue;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(630, 50, 300, 450));
            GUILayout.Label("<b>CONDITIONAL TIMERS</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Condition flag control
            GUILayout.Label($"Condition Flag: {_conditionFlag}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set True"))
            {
                _conditionFlag = true;
                Debug.Log("[Condition] Flag set to TRUE");
            }
            if (GUILayout.Button("Set False"))
            {
                _conditionFlag = false;
                Debug.Log("[Condition] Flag set to FALSE");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // WaitUntil (flag)
            GUILayout.Label($"WaitUntil (flag == true) - Waiting: {_isWaitingUntil}");
            if (GUILayout.Button("WaitUntil Flag is True"))
            {
                if (_waitUntilHandle.IsValid) MxTimer.Cancel(_waitUntilHandle);
                _isWaitingUntil = true;
                _waitUntilHandle = MxTimer.WaitUntil(
                    () => _conditionFlag,
                    () =>
                    {
                        _isWaitingUntil = false;
                        Debug.Log("[WaitUntil] Condition met! Flag is now true.");
                    },
                    this
                );
                Debug.Log("[WaitUntil] Started waiting for flag to be true...");
            }
            GUILayout.Space(5);

            // WaitWhile (flag)
            GUILayout.Label($"WaitWhile (flag == true) - Waiting: {_isWaitingWhile}");
            if (GUILayout.Button("WaitWhile Flag is True"))
            {
                if (_waitWhileHandle.IsValid) MxTimer.Cancel(_waitWhileHandle);
                _isWaitingWhile = true;
                _waitWhileHandle = MxTimer.WaitWhile(
                    () => _conditionFlag,
                    () =>
                    {
                        _isWaitingWhile = false;
                        Debug.Log("[WaitWhile] Condition ended! Flag is now false.");
                    },
                    this
                );
                Debug.Log("[WaitWhile] Started waiting while flag is true...");
            }
            GUILayout.Space(15);

            // Value-based condition
            GUILayout.Label($"Current Value: {_currentValue} / Target: {_targetValue}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1"))
            {
                _currentValue++;
                Debug.Log($"[Value] Incremented to {_currentValue}");
            }
            if (GUILayout.Button("+5"))
            {
                _currentValue += 5;
                Debug.Log($"[Value] Incremented to {_currentValue}");
            }
            if (GUILayout.Button("Reset"))
            {
                _currentValue = 0;
                Debug.Log("[Value] Reset to 0");
            }
            GUILayout.EndHorizontal();

            GUILayout.Label($"WaitUntil (value >= {_targetValue}) - Waiting: {_isWaitingUntilValue}");
            if (GUILayout.Button($"WaitUntil Value >= {_targetValue}"))
            {
                if (_waitUntilValueHandle.IsValid) MxTimer.Cancel(_waitUntilValueHandle);
                _isWaitingUntilValue = true;
                _waitUntilValueHandle = MxTimer.WaitUntil(
                    () => _currentValue >= _targetValue,
                    () =>
                    {
                        _isWaitingUntilValue = false;
                        Debug.Log($"[WaitUntil] Value reached {_currentValue} (target was {_targetValue})");
                    },
                    this
                );
                Debug.Log($"[WaitUntil] Started waiting for value >= {_targetValue}...");
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
