using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleConfirmDialog : MxDialog
    {
        private readonly Label _title;
        private readonly Label _message;
        private readonly Button _confirmButton;
        private readonly Action _onConfirm;

        public SampleConfirmDialog(string title, string message, Action onConfirm)
        {
            _onConfirm = onConfirm;

            AddToClassList("mx-demo-dialog-overlay");

            VisualElement dialogPanel = new VisualElement();
            dialogPanel.AddToClassList("mx-demo-dialog-panel");
            Add(dialogPanel);

            _title = new Label();
            _title.AddToClassList("mx-demo-dialog-title");

            _message = new Label();
            _message.AddToClassList("mx-demo-dialog-message");

            dialogPanel.Add(_title);
            dialogPanel.Add(_message);

            VisualElement row = new VisualElement();
            row.AddToClassList("mx-demo-dialog-row");

            _confirmButton = new Button(OnConfirm) { text = "OK" };
            Button cancelButton = new Button(Close) { text = "Cancel" };
            row.Add(_confirmButton);
            row.Add(cancelButton);
            dialogPanel.Add(row);

            Init(title, message);
        }

        public override void Init(string title, string message)
        {
            _title.text = title;
            _message.text = message;
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
