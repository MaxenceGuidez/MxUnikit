using System.Collections.Generic;
using MxUnikit.Log;

namespace MxUnikit.UI
{
    public abstract class MxMenuUiManager<TKey> : MxUiManager
    {
        private readonly Dictionary<TKey, MxMenu> _menus = new Dictionary<TKey, MxMenu>();
        private readonly Stack<TKey> _history = new Stack<TKey>();

        private MxMenu _current;
        private TKey _currentKey;
        private bool _hasCurrent;

        public bool HasMenuOpen => _hasCurrent;
        public bool HasHistory => _history.Count > 0;

        #region Registration

        protected void RegisterMenu(TKey key, MxMenu menu)
        {
            if (menu == null)
            {
                MxLog.W($"Cannot register a null menu for key '{key}'.");
                return;
            }

            _menus[key] = menu;
            menu.Hide();
        }

        #endregion

        #region Navigation

        public void ShowMenu(TKey key, bool isRoot = false)
        {
            if (!_menus.TryGetValue(key, out MxMenu menu))
            {
                MxLog.W($"Menu '{key}' is not registered.");
                return;
            }

            if (_hasCurrent && !EqualityComparer<TKey>.Default.Equals(_currentKey, key))
            {
                if (isRoot)
                {
                    _history.Clear();
                }
                else
                {
                    _history.Push(_currentKey);
                }
            }

            ShowInternal(key, menu);
        }

        public void OnClickBack()
        {
            if (_history.Count > 0)
            {
                TKey previous = _history.Pop();
                if (_menus.TryGetValue(previous, out MxMenu menu))
                {
                    ShowInternal(previous, menu);
                    return;
                }
            }

            HideAllMenus();
        }

        public void HideAllMenus()
        {
            foreach (MxMenu menu in _menus.Values)
            {
                menu.Hide();
            }

            _current = null;
            _hasCurrent = false;
            _history.Clear();

            OnAllMenusHidden();
        }

        private void ShowInternal(TKey key, MxMenu menu)
        {
            if (menu == _current) return;

            _current?.Hide();

            menu.Show();
            _current = menu;
            _currentKey = key;
            _hasCurrent = true;

            OnMenuShown(key, menu);
        }

        #endregion

        #region Hooks

        protected virtual void OnMenuShown(TKey key, MxMenu menu) { }
        protected virtual void OnAllMenusHidden() { }

        #endregion
    }
}
