using System;
using System.Threading.Tasks;
using UnityEngine;
using MxUnikit.Log;
using MxUnikit.Provider;

namespace MxUnikit.Core
{
    public abstract class MxCoreManager : MonoBehaviour
    {
        public event Action OnInitialized;

        private MxBootstrapper _bootstrapper;

        #region Init

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            MxProvider.Register(this);
        }

        private void OnDestroy()
        {
            MxProvider.Unregister<MxCoreManager>();

            if (_bootstrapper != null)
            {
                _bootstrapper.OnPreloaded -= OnBootstrapperPreloaded;
            }
        }

        private void Start()
        {
            if (!MxProvider.TryGet(out _bootstrapper))
            {
                MxLog.E("No MxBootstrapper registered.");
                return;
            }

            _bootstrapper.OnPreloaded += OnBootstrapperPreloaded;
        }

        private async void OnBootstrapperPreloaded()
        {
            _bootstrapper.OnPreloaded -= OnBootstrapperPreloaded;
            await InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Initialize();
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
