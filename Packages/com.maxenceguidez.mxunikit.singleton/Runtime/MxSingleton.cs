using System;

namespace MxUnikit.Singleton
{
	public abstract class MxSingleton<T> where T : MxSingleton<T>, new()
	{
		private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() =>
		{
			T instance = new T();
			instance.OnInit();
			return instance;
		});

		public static T Instance => _lazyInstance.Value;

		public static bool IsInitialized => _lazyInstance.IsValueCreated;

		protected virtual void OnInit() { }

		public static void EagerInitialize()
		{
			T _ = Instance;
		}
	}
}
