using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MxUnikit.Tests.Editor
{
    public static class PrefabScanner
    {
        public static List<ValidationException> ValidateAllPrefabs()
        {
            List<ValidationException> exceptions = new List<ValidationException>();
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    exceptions.AddRange(RequiredFieldValidator.ValidateGameObject(prefabRoot, path));
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            return exceptions;
        }
    }
}
