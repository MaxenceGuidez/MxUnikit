using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core
{
    public abstract class MxBootstrapper : MonoBehaviour
    {
        [SerializeField] private string _nextSceneName;

        public bool IsPreloaded { get; private set; }

        public event Action OnPreloaded;

        #region Init

        protected virtual void Awake()
        {
            MxProvider.Register(this);
        }

        protected virtual void OnDestroy()
        {
            MxProvider.Unregister<MxBootstrapper>();
        }

        private async void Start()
        {
            try
            {
                await Preload();

                IsPreloaded = true;
                OnPreloaded?.Invoke();

                LoadNextScene();
            }
            catch (Exception ex)
            {
                MxLog.Ex(ex);
            }
        }

        protected abstract Task Preload();

        private void LoadNextScene()
        {
            if (string.IsNullOrEmpty(_nextSceneName))
            {
                MxLog.E("Next scene name is not set on MxBootstrapper.");
                return;
            }

            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }

        #endregion
    }
}
