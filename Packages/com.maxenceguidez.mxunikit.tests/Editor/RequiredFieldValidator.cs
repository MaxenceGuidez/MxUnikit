using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MxUnikit.Tests.Editor
{
    public static class RequiredFieldValidator
    {
        #region Validation

        public static List<ValidationException> ValidateGameObject(GameObject gameObject, string assetPath)
        {
            List<ValidationException> exceptions = new List<ValidationException>();
            Component[] components = gameObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                ValidateComponent(component, assetPath, GetGameObjectPath(gameObject), exceptions);
            }

            foreach (Transform child in gameObject.transform)
            {
                exceptions.AddRange(ValidateGameObject(child.gameObject, assetPath));
            }

            return exceptions;
        }

        private static void ValidateComponent(Component component, string assetPath, string gameObjectPath, List<ValidationException> exceptions)
        {
            Type type = component.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (Attribute.GetCustomAttribute(field, typeof(RequiredFieldAttribute)) is not RequiredFieldAttribute) continue;

                if (!IsReferenceType(field.FieldType)) continue;

                object value = field.GetValue(component);

                if (IsUnityObjectNull(value))
                {
                    exceptions.Add(new ValidationException(assetPath, gameObjectPath, type.Name, field.Name));
                }
            }
        }

        #endregion

        #region Utils

        private static bool IsUnityObjectNull(object obj)
        {
            return obj switch
            {
                null => true,
                Object unityObj => !unityObj,
                _ => false
            };
        }

        private static bool IsReferenceType(Type type)
        {
            return !type.IsValueType;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        #endregion
    }
}
