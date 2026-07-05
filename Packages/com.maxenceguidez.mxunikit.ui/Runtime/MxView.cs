using UnityEngine.UIElements;

namespace MxUnikit.UI
{
    public abstract class MxView : VisualElement
    {
        public bool IsVisible => !ClassListContains(MxStyles.HiddenClassName);

        protected MxView()
        {
            AddToClassList(MxStyles.ViewClassName);
        }

        public void Show()
        {
            RemoveFromClassList(MxStyles.HiddenClassName);
            OnShow();
        }

        public void Hide()
        {
            AddToClassList(MxStyles.HiddenClassName);
            OnHide();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
