using System.Collections.Generic;
using MxUnikit.Log;

namespace MxUnikit.UI
{
    public abstract class MxOverlayUiManager : MxUiManager
    {
        private readonly Stack<MxDialog> _dialogs = new Stack<MxDialog>();

        public bool IsAnyDialogOpen => _dialogs.Count > 0;
        public MxDialog TopDialog => _dialogs.Count > 0 ? _dialogs.Peek() : null;

        #region Dialogs

        public void ShowDialog(MxDialog dialog)
        {
            if (dialog == null) return;

            if (_root == null)
            {
                MxLog.W("Cannot show a dialog before the panel is ready.");
                return;
            }

            if (_dialogs.Count == 0)
            {
                OnFirstDialogOpened();
            }

            dialog.CloseRequested += () => CloseDialog(dialog);

            _root.Add(dialog);
            _dialogs.Push(dialog);

            dialog.Show();
        }

        public void CloseDialog(MxDialog dialog)
        {
            if (_dialogs.Count == 0 || _dialogs.Peek() != dialog) return;

            _dialogs.Pop();

            if (_root != null && _root.Contains(dialog))
            {
                _root.Remove(dialog);
            }

            if (_dialogs.Count == 0)
            {
                OnLastDialogClosed();
            }
            else
            {
                _dialogs.Peek().Show();
            }
        }

        public void CloseAllDialogs()
        {
            if (_dialogs.Count == 0) return;

            while (_dialogs.Count > 0)
            {
                MxDialog dialog = _dialogs.Pop();
                if (_root != null && _root.Contains(dialog))
                {
                    _root.Remove(dialog);
                }
            }

            OnLastDialogClosed();
        }

        #endregion

        #region Hooks

        protected virtual void OnFirstDialogOpened() { }
        protected virtual void OnLastDialogClosed() { }

        #endregion
    }
}
