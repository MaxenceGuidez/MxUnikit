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

        public void Close()
        {
            CloseRequested?.Invoke();
        }
    }
}
