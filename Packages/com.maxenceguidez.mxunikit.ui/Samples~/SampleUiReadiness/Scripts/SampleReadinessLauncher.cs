using UnityEngine;
using MxUnikit.Log;

namespace MxUnikit.UI.Samples.SampleUiReadiness
{
    public class SampleReadinessLauncher : MonoBehaviour
    {
        [SerializeField] private SampleReadinessUiManager _uiManager;

        private async void Start()
        {
            try
            {
                MxLog.L($"Before await: IsUiReady = {_uiManager.IsUiReady}.");

                await _uiManager.WaitUntilUiReadyAsync();

                MxLog.L($"After await: IsUiReady = {_uiManager.IsUiReady}.");
                _uiManager.StatusLabel.text = "Ready! Label updated from Start() after WaitUntilUiReadyAsync().";
            }
            catch (Exception e)
            {
                MxLog.Ex(e);
            }
        }
    }
}
