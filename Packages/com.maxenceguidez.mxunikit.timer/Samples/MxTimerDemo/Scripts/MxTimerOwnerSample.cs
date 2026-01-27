using System;
using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Tests: Owner-based control (CancelAll, PauseAll, ResumeAll, CountFor)
    /// Simulates multiple "entities" with their own timers
    /// </summary>
    public class MxTimerOwnerSample : MonoBehaviour
    {
        private readonly object _ownerA = new object();
        private readonly object _ownerB = new object();
        private readonly object _ownerC = new object();

        private int _tickA;
        private int _tickB;
        private int _tickC;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(630, 500, 300, 500));
            GUILayout.Label("<b>OWNER-BASED CONTROL</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16 });
            GUILayout.Space(10);

            // Owner A
            DrawOwnerSection("Owner A", _ownerA, ref _tickA, Color.red);
            GUILayout.Space(5);

            // Owner B
            DrawOwnerSection("Owner B", _ownerB, ref _tickB, Color.green);
            GUILayout.Space(5);

            // Owner C
            DrawOwnerSection("Owner C", _ownerC, ref _tickC, Color.blue);
            GUILayout.Space(15);

            // Bulk operations
            GUILayout.Label("Bulk Operations:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create All"))
            {
                CreateTimersFor(_ownerA, "A");
                CreateTimersFor(_ownerB, "B");
                CreateTimersFor(_ownerC, "C");
            }
            if (GUILayout.Button("Cancel All"))
            {
                MxTimer.CancelAll(_ownerA);
                MxTimer.CancelAll(_ownerB);
                MxTimer.CancelAll(_ownerC);
                Debug.Log("[Owner] All timers cancelled");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label($"Total active timers: {MxTimer.ActiveCount}");
            GUILayout.Label($"Owner A count: {MxTimer.CountFor(_ownerA)}");
            GUILayout.Label($"Owner B count: {MxTimer.CountFor(_ownerB)}");
            GUILayout.Label($"Owner C count: {MxTimer.CountFor(_ownerC)}");

            GUILayout.EndArea();
        }

        private void DrawOwnerSection(string ownerName, object owner, ref int tick, Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            GUILayout.Label($"{ownerName} - Ticks: {tick}, Active: {MxTimer.CountFor(owner)}");
            GUI.color = oldColor;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                CreateTimersFor(owner, ownerName);
            }
            if (GUILayout.Button("Pause", GUILayout.Width(60)))
            {
                MxTimer.PauseAll(owner);
                Debug.Log($"[{ownerName}] All timers paused");
            }
            if (GUILayout.Button("Resume", GUILayout.Width(60)))
            {
                MxTimer.ResumeAll(owner);
                Debug.Log($"[{ownerName}] All timers resumed");
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                MxTimer.CancelAll(owner);
                Debug.Log($"[{ownerName}] All timers cancelled");
            }
            GUILayout.EndHorizontal();
        }

        private void CreateTimersFor(object owner, string ownerName)
        {
            MxTimer.CancelAll(owner);

            if (owner == _ownerA) _tickA = 0;
            else if (owner == _ownerB) _tickB = 0;
            else _tickC = 0;

            Action incrementTick =
                owner == _ownerA ? () => _tickA++ :
                owner == _ownerB ? () => _tickB++ :
                () => _tickC++;

            MxTimer.Repeat(0.5f, () =>
            {
                incrementTick();
                Debug.Log($"[{ownerName}] Timer 1 tick");
            }, owner);

            MxTimer.Repeat(0.7f, () =>
            {
                incrementTick();
                Debug.Log($"[{ownerName}] Timer 2 tick");
            }, owner);

            MxTimer.Repeat(1.2f, () =>
            {
                incrementTick();
                Debug.Log($"[{ownerName}] Timer 3 tick");
            }, owner);

            Debug.Log($"[{ownerName}] Created 3 timers");
        }

        private void OnDestroy()
        {
            MxTimer.CancelAll(_ownerA);
            MxTimer.CancelAll(_ownerB);
            MxTimer.CancelAll(_ownerC);
        }
    }
}
