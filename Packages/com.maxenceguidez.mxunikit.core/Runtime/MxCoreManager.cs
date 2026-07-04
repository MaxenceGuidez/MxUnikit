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

        #region Init

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            MxProvider.Register(this);
        }

        private void OnDestroy()
        {
            MxProvider.Unregister<MxCoreManager>();
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
