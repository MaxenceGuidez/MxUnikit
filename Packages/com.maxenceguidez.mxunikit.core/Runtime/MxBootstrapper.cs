using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core
{
    public class MxBootstrapper : MonoBehaviour
    {
        [SerializeField] private string _nextSceneName;

        private MxCoreManager _core;
        private bool _hasLoadedNextScene;

        private async void Start()
        {
            _core = MxProvider.Get<MxCoreManager>();

            if (_core == null)
            {
                MxLog.E("No MxCoreManager registered. Loading next scene without initialization.");
                LoadNextScene();
                return;
            }

            try
            {
                _core.OnInitialized += OnCoreInitialized;
                await _core.InitializeAsync();
            }
            catch (Exception ex)
            {
                MxLog.E($"Error during initialization: {ex.Message}");
                LoadNextScene();
            }
        }

        private void OnDestroy()
        {
            if (_core != null)
            {
                _core.OnInitialized -= OnCoreInitialized;
            }
        }

        private void OnCoreInitialized() => LoadNextScene();

        private void LoadNextScene()
        {
            if (_hasLoadedNextScene)
            {
                MxLog.W("Next scene already loaded. Ignoring duplicate request.");
                return;
            }

            if (string.IsNullOrEmpty(_nextSceneName))
            {
                MxLog.E("Next scene name is not set on MxBootstrapper.");
                return;
            }

            _hasLoadedNextScene = true;
            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }
    }
}
