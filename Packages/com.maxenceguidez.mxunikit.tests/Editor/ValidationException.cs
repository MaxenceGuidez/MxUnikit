using System;

namespace MxUnikit.Tests.Editor
{
    public class ValidationException : Exception
    {
        public string AssetPath { get; }
        public string GameObjectPath { get; }
        public string ComponentType { get; }
        public string FieldName { get; }

        public ValidationException(string assetPath, string gameObjectPath, string componentType, string fieldName)
            : base($"Required field '{fieldName}' is null in {componentType} on '{gameObjectPath}' (Asset: {assetPath})")
        {
            AssetPath = assetPath;
            GameObjectPath = gameObjectPath;
            ComponentType = componentType;
            FieldName = fieldName;
        }
    }
}
