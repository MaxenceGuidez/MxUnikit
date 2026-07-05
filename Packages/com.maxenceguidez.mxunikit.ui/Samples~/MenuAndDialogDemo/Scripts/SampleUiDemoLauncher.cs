using UnityEngine;

namespace MxUnikit.UI.Samples
{
    public class SampleUiDemoLauncher : MonoBehaviour
    {
        [SerializeField] private SampleMenuUiManager _menuUiManager;
        [SerializeField] private SampleOverlayUiManager _overlayUiManager;

        private bool _isWired;

        private void Update()
        {
            if (_isWired) return;
            if (_menuUiManager == null || _menuUiManager.HomeMenu == null) return;

            _menuUiManager.HomeMenu.OnClickPlay += _menuUiManager.StartGameplay;
            _menuUiManager.HomeMenu.OnClickQuit += ConfirmOnClickQuit;

            _isWired = true;
        }

        private void ConfirmOnClickQuit()
        {
            SampleConfirmDialog dialog = new SampleConfirmDialog(
                "Quit?",
                "Are you sure you want to quit?",
                Application.Quit
            );

            _overlayUiManager.ShowDialog(dialog);
        }
    }
}
