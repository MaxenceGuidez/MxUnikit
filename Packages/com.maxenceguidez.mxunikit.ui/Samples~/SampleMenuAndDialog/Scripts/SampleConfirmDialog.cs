using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples.SampleMenuAndDialog
{
    [UxmlElement]
    public partial class SampleConfirmDialog : SampleMessageDialog
    {
        private Action _onCancel;

        public SampleConfirmDialog()
        {
            _dialogPanel.Add(new Button(OnClickCancel) { text = "Cancel" });
        }

        public void Init(string title, string message, Action onConfirm, Action onCancel = null)
        {
            base.Init(title, message, onConfirm);
            _onCancel = onCancel;
        }

        private void OnClickCancel()
        {
            _onCancel?.Invoke();
            Close();
        }
    }
}
