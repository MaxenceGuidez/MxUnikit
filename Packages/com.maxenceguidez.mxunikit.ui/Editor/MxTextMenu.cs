using UnityEditor;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MxUnikit.UI.Editor
{
    public static class MxTextMenu
    {
        // ----------------------------------------------------------------------------------------
        [MenuItem("GameObject/UI/MxUnikit/MxText")]
        private static void CreateMxText(MenuCommand menuCommand)
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGo = new GameObject("Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();

                Undo.RegisterCreatedObjectUndo(canvasGo, "Create " + canvasGo.name);
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemGo = new GameObject("EventSystem", typeof(EventSystem));
                InputModuleComponentFactory.AddInputModule(eventSystemGo);

                Undo.RegisterCreatedObjectUndo(eventSystemGo, "Create " + eventSystemGo.name);
            }

            GameObject textGo = new GameObject("MxText", typeof(RectTransform), typeof(MxText));
            GameObjectUtility.SetParentAndAlign(textGo, canvas.gameObject);

            Undo.RegisterCreatedObjectUndo(textGo, "Create " + textGo.name);
            Selection.activeObject = textGo;
        }

        // ----------------------------------------------------------------------------------------
    }
}