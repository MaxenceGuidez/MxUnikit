using System;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples.SampleMenuAndDialog
{
    [UxmlElement]
    public partial class SampleMessageDialog : MxDialog
    {
        protected readonly VisualElement _dialogPanel;

        private readonly Label _labelTitle;
        private readonly Label _labelMessage;
        private readonly Button _buttonConfirm;

        private Action _onConfirm;

        public SampleMessageDialog()
        {
            AddToClassList("mx-sample-dialog-overlay");

            _labelTitle = new Label();
            _labelTitle.AddToClassList("mx-sample-dialog-title");

            _labelMessage = new Label();
            _labelMessage.AddToClassList("mx-sample-dialog-message");

            _buttonConfirm = new Button(OnClickConfirm) { text = "OK" };

            _dialogPanel = new VisualElement();
            _dialogPanel.AddToClassList("mx-sample-dialog-panel");
            _dialogPanel.Add(_labelTitle);
            _dialogPanel.Add(_labelMessage);
            _dialogPanel.Add(_buttonConfirm);

            Add(_dialogPanel);
        }

        public void Init(string title, string message, Action onConfirm = null)
        {
            _labelTitle.text = title;
            _labelMessage.text = message;
            _onConfirm = onConfirm;
        }

        protected override void OnShow()
        {
            _buttonConfirm.Focus();
        }

        private void OnClickConfirm()
        {
            _onConfirm?.Invoke();
            Close();
        }
    }
}
