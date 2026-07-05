using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleConfirmDialog : MxDialog
    {
        private readonly Button _confirmButton;
        private readonly Action _onConfirm;

        public SampleConfirmDialog(string title, string message, Action onConfirm)
        {
            _onConfirm = onConfirm;

            AddToClassList("mx-demo-dialog-overlay");

            VisualElement dialogPanel = new VisualElement();
            dialogPanel.AddToClassList("mx-demo-dialog-panel");
            Add(dialogPanel);

            Label labelTitle = new Label();
            labelTitle.AddToClassList("mx-demo-dialog-title");
            labelTitle.text = title;

            Label labelMessage = new Label();
            labelMessage.AddToClassList("mx-demo-dialog-message");
            labelMessage.text = message;

            VisualElement row = new VisualElement();
            row.AddToClassList("mx-demo-dialog-row");

            _confirmButton = new Button(OnConfirm) { text = "OK" };
            Button cancelButton = new Button(Close) { text = "Cancel" };
            row.Add(_confirmButton);
            row.Add(cancelButton);

            dialogPanel.Add(labelTitle);
            dialogPanel.Add(labelMessage);
            dialogPanel.Add(row);
        }

        protected override void OnShow()
        {
            _confirmButton.Focus();
        }

        private void OnConfirm()
        {
            _onConfirm?.Invoke();
            Close();
        }
    }
}
