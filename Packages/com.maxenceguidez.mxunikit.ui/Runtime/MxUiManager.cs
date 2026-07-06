using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace MxUnikit.UI
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class MxUiManager : MonoBehaviour
    {
        protected PanelRenderer _panelRenderer;
        protected VisualElement _root;

        private readonly TaskCompletionSource<bool> _uiReadyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public VisualElement Root => _root;
        public Focusable FocusedElement => _root?.focusController?.focusedElement;
        public bool IsVisible => _root != null && !_root.ClassListContains(MxStyles.HiddenClassName);
        public bool IsUiReady => _uiReadyTcs.Task.IsCompletedSuccessfully;

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

        protected virtual void OnDestroy()
        {
            _uiReadyTcs.TrySetCanceled();
        }

        private void OnUiReload(PanelRenderer renderer, VisualElement root)
        {
            _root = root;
            OnUiReady();

            _uiReadyTcs.TrySetResult(true);
        }

        protected virtual void OnUiReady() { }

        public Task WaitUntilUiReadyAsync()
        {
            return _uiReadyTcs.Task;
        }

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
