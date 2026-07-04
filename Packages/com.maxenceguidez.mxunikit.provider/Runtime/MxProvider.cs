using System;
using System.Collections.Generic;
using UnityEngine;
using MxUnikit.Log;

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
                MxLog.E($"Provided instance for '{type.Name}' is null.");
                return;
            }

            if (_instances.TryAdd(type, instance))
            {
                MxLog.L($"Registered '{type.Name}'.");
            }
            else
            {
                MxLog.W($"'{type.Name}' is already registered. Replacing existing instance.");
                _instances[type] = instance;
            }
        }

        public static void Unregister<T>() where T : class
        {
            Type type = typeof(T);

            if (_instances.Remove(type))
            {
                MxLog.L($"Unregistered '{type.Name}'.");
            }
            else
            {
                MxLog.W($"'{type.Name}' was not registered.");
            }
        }

        public static bool IsRegistered<T>() where T : class
        {
            return _instances.ContainsKey(typeof(T));
        }

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
                MxLog.E($"'{typeof(T).Name}' not registered!");
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

            MxLog.L($"Cleared all instances ({count} total).");
        }
    }
}
