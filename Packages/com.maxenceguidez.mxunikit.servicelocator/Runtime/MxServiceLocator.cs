using System;
using System.Collections.Generic;
using UnityEngine;

namespace MxUnikit.ServiceLocator
{
	public static class MxServiceLocator
	{
		private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        #region Registration

        public static void Register<T>(T instance) where T : class, IService
        {
            Type type = typeof(T);

            if (_services.TryAdd(type, instance))
            {
                Debug.Log($"[MxServiceLocator] Registered service '{type.Name}'.");
            }
            else
            {
                Debug.LogWarning($"[MxServiceLocator] Service '{type.Name}' is already registered. Replacing existing instance.");
                _services[type] = instance;
            }
        }

        public static void Unregister<T>() where T : class, IService
        {
            Type type = typeof(T);

            if (_services.Remove(type))
            {
                Debug.Log($"[MxServiceLocator] Unregistered service '{type.Name}'.");
            }
            else
            {
                Debug.LogWarning($"[MxServiceLocator] Service '{type.Name}' was not registered.");
            }
        }

        public static bool IsRegistered<T>() where T : class, IService
        {
            return _services.ContainsKey(typeof(T));
        }

        #endregion

        #region Getters

        public static T Get<T>() where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out IService service))
            {
                return service as T;
            }

            return null;
        }

        public static bool TryGet<T>(out T service) where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out IService existingService))
            {
                service = existingService as T;
                return true;
            }

            service = null;
            return false;
        }

        public static IReadOnlyDictionary<Type, IService> GetAllServices()
        {
            return new Dictionary<Type, IService>(_services);
        }

        #endregion

		public static void Clear()
		{
            int count = _services.Count;
            _services.Clear();

            Debug.Log($"[MxServiceLocator] Cleared all services ({count} total).");
		}
	}
}
