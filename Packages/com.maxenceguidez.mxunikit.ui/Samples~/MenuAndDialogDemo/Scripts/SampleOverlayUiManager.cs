using System;
using MxUnikit.Log;

namespace MxUnikit.UI.Samples
{
    public class SampleOverlayUiManager : MxOverlayUiManager
    {
        protected override void OnFirstDialogOpened()
        {
            MxLog.L("Would disable gameplay input here.");
        }

        protected override void OnLastDialogClosed()
        {
            MxLog.L("Would restore gameplay input here.");
        }

        #region Dialogs

        public void ShowMessageDialog(string title, string message, Action onConfirm = null)
        {
            SampleMessageDialog dialog = new SampleMessageDialog();
            dialog.Init(title, message, onConfirm);
            ShowDialog(dialog);
        }

        public void ShowConfirmDialog(string title, string message, Action onConfirm = null, Action onCancel = null)
        {
            SampleConfirmDialog dialog = new SampleConfirmDialog();
            dialog.Init(title, message, onConfirm, onCancel);
            ShowDialog(dialog);
        }

        #endregion
    }
}
