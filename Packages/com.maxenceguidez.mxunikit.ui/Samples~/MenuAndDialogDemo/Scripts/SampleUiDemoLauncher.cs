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
            _menuUiManager.HomeMenu.OnClickInfos += OnClickInfos;
            _menuUiManager.HomeMenu.OnClickQuit += OnClickQuit;

            _isWired = true;
        }

        private void OnClickInfos()
        {
            _overlayUiManager.ShowMessageDialog("Infos", "This is an information.");
        }

        private void OnClickQuit()
        {
            _overlayUiManager.ShowConfirmDialog(
                "Quit?",
                "Are you sure you want to quit?",
                Application.Quit
            );
        }
    }
}
