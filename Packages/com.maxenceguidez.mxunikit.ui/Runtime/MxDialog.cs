using System;

namespace MxUnikit.UI
{
    public abstract class MxDialog : MxView
    {
        public event Action CloseRequested;

        protected MxDialog()
        {
            AddToClassList(MxStyles.DialogClassName);
        }

        public abstract void Init(string title, string message);

        public void Close()
        {
            CloseRequested?.Invoke();
        }
    }
}
