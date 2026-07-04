using System;
using System.Collections.Generic;
using UnityEngine;

namespace MxUnikit.Provider
{
    public static class MxProvider
    {
        private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private static bool _isQuitting;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _instances.Clear();
            _isQuitting = false;
            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }

        private static void OnQuitting()
        {
            _isQuitting = true;
        }

        #region Registration

        public static void Register<T>(T instance) where T : class
        {
            Type type = typeof(T);

            if (instance == null)
            {
                Debug.LogError($"[MxProvider] Provided instance for '{type.Name}' is null.");
                return;
            }

            if (_instances.TryAdd(type, instance))
            {
                Debug.Log($"[MxProvider] Registered '{type.Name}'.");
            }
            else
            {
                Debug.LogWarning($"[MxProvider] '{type.Name}' is already registered. Replacing existing instance.");
                _instances[type] = instance;
            }
        }

        public static void Unregister<T>() where T : class
        {
            Type type = typeof(T);

            if (_instances.Remove(type))
            {
                Debug.Log($"[MxProvider] Unregistered '{type.Name}'.");
            }
            else
            {
                Debug.LogWarning($"[MxProvider] '{type.Name}' was not registered.");
            }
        }

        public static bool IsRegistered<T>() where T : class => _instances.ContainsKey(typeof(T));

        #endregion

        #region Getters

        public static T Get<T>() where T : class
        {
            if (_instances.TryGetValue(typeof(T), out object instance))
            {
                return instance as T;
            }

            if (!_isQuitting)
            {
                Debug.LogError($"[MxProvider] '{typeof(T).Name}' not registered!");
            }

            return null;
        }

        public static bool TryGet<T>(out T instance) where T : class
        {
            if (_instances.TryGetValue(typeof(T), out object existing))
            {
                instance = existing as T;
                return true;
            }

            instance = null;
            return false;
        }

        public static IReadOnlyDictionary<Type, object> GetAll()
        {
            return new Dictionary<Type, object>(_instances);
        }

        #endregion

        public static void Clear()
        {
            int count = _instances.Count;
            _instances.Clear();

            Debug.Log($"[MxProvider] Cleared all instances ({count} total).");
        }
    }
}
