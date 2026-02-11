using UnityEngine;

namespace MxUnikit.Singleton
{
	public abstract class MxMonoSingleton<T> : MonoBehaviour where T : MxMonoSingleton<T>
	{
		private static T _instance;
		private static bool _isQuitting;
		private static readonly object _lock = new object();

		protected virtual MxSingletonMode Mode => MxSingletonMode.Persistent;

		public static T Instance
		{
			get
			{
				if (_isQuitting)
				{
					Debug.LogWarning($"[MxMonoSingleton] Instance of '{typeof(T)}' requested during application quit. Returning null.");
					return null;
				}

				if (_instance != null) return _instance;

				lock (_lock)
				{
					if (_instance != null) return _instance;

					_instance = FindFirstObjectByType<T>();

					if (_instance == null)
					{
						GameObject go = new GameObject($"[{typeof(T).Name}]");
						_instance = go.AddComponent<T>();
					}

					if (_instance.Mode == MxSingletonMode.Persistent)
					{
						DontDestroyOnLoad(_instance.gameObject);
					}

					_instance.OnAwake();
				}

				return _instance;
			}
		}

		public static bool HasInstance => _instance != null;
		public static bool IsQuitting => _isQuitting;

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;

				if (Mode == MxSingletonMode.Persistent)
				{
					DontDestroyOnLoad(gameObject);
				}

				OnAwake();
			}
			else if (_instance != this)
			{
				Debug.LogWarning($"[MxMonoSingleton] Duplicate instance of '{typeof(T)}' found. Destroying duplicate.");
				Destroy(gameObject);
			}
		}

		protected virtual void OnDestroy()
        {
            if (_instance != this) return;

            OnDestroySingleton();
            _instance = null;
        }

		protected virtual void OnApplicationQuit()
		{
			_isQuitting = true;
		}

		protected virtual void OnAwake() { }
		protected virtual void OnDestroySingleton() { }

		public static void DestroySingleton()
        {
            if (_instance == null) return;

            Destroy(_instance.gameObject);
            _instance = null;
        }
	}
}
