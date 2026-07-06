using System;
using System.Threading.Tasks;
using UnityEngine;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core
{
    public abstract class MxCoreManager : MonoBehaviour
    {
        private MxBootstrapper _bootstrapper;

        public bool IsInitialized { get; private set; }

        public event Action OnInitialized;

        #region Init

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            MxProvider.Register(this);
        }

        protected virtual void OnDestroy()
        {
            MxProvider.Unregister<MxCoreManager>();

            if (_bootstrapper != null)
            {
                _bootstrapper.OnPreloaded -= HandleBootstrapperPreloaded;
            }
        }

        private void Start()
        {
            if (!MxProvider.TryGet(out _bootstrapper))
            {
                MxLog.E("No MxBootstrapper registered.");
                return;
            }

            if (_bootstrapper.IsPreloaded)
            {
                _ = InitializeAsync();
                return;
            }

            _bootstrapper.OnPreloaded += HandleBootstrapperPreloaded;
        }

        private void HandleBootstrapperPreloaded()
        {
            _bootstrapper.OnPreloaded -= HandleBootstrapperPreloaded;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await Initialize();

                IsInitialized = true;
                OnInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                MxLog.Ex(ex);
            }
        }

        protected abstract Task Initialize();

        #endregion
    }
}
