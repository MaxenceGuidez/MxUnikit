using UnityEngine;
using UnityEngine.UIElements;

namespace MxUnikit.UI
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class MxUiManager : MonoBehaviour
    {
        protected PanelRenderer _panelRenderer;
        protected VisualElement _root;

        public VisualElement Root => _root;
        public Focusable FocusedElement => _root?.focusController?.focusedElement;
        public bool IsVisible => _root != null && !_root.ClassListContains(MxStyles.HiddenClassName);

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _panelRenderer = GetComponent<PanelRenderer>();
        }

        protected virtual void OnEnable()
        {
            _panelRenderer.RegisterUIReloadCallback(OnUiReload);
        }

        protected virtual void OnDisable()
        {
            _panelRenderer.UnregisterUIReloadCallback(OnUiReload);
        }

        private void OnUiReload(PanelRenderer renderer, VisualElement root)
        {
            _root = root;
            OnUiReady();
        }

        protected virtual void OnUiReady() { }

        public void Show()
        {
            _root?.RemoveFromClassList(MxStyles.HiddenClassName);
        }

        public void Hide()
        {
            _root?.AddToClassList(MxStyles.HiddenClassName);
        }

        public void SetRootEnabled(bool isEnabled)
        {
            _root?.SetEnabled(isEnabled);
        }
    }
}
