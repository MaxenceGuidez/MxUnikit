using UnityEngine;

namespace MxUnikit.Timer.Samples
{
    /// <summary>
    /// Main demo manager that sets up the scene and provides common UI elements.
    /// Attach all sample scripts to this GameObject.
    /// </summary>
    public class MxTimerDemoManager : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void OnGUI()
        {
            // Title bar
            GUI.Box(new Rect(0, 0, Screen.width, 30), "");
            GUI.Label(new Rect(10, 5, 400, 25), "<b>MxTimer Demo - All Features</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 18 });

            // Instructions
            GUI.Label(new Rect(Screen.width - 300, 5, 290, 25), "Check Console for debug logs", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight });
        }
    }
}
