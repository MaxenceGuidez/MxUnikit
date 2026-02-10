using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using NUnit.Framework;

namespace MxUnikit.Tests.Editor
{
    public class ProjectReferenceValidationTests
    {
        #region Tests

        [Test]
        public void ValidateAllPrefabs_NoMissingRequiredFields()
        {
            List<ValidationException> exceptions = ValidateAllPrefabs();

            if (exceptions.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Found {exceptions.Count} missing required field(s) in prefabs:");

                foreach (ValidationException ex in exceptions)
                {
                    errorMessage.AppendLine($"  - {ex.Message}");
                }

                Assert.Fail(errorMessage.ToString());
            }

            Assert.Pass("Validated all prefabs successfully. No missing required fields.");
        }

        [Test]
        public void ValidateAllScenes_NoMissingRequiredFields()
        {
            List<ValidationException> exceptions = ValidateAllScenes();

            if (exceptions.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Found {exceptions.Count} missing required field(s) in scenes:");

                foreach (ValidationException ex in exceptions)
                {
                    errorMessage.AppendLine($"  - {ex.Message}");
                }

                Assert.Fail(errorMessage.ToString());
            }

            Assert.Pass("Validated all scenes successfully. No missing required fields.");
        }

        #endregion

        #region Validation

        private static List<ValidationException> ValidateAllPrefabs()
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

        private static List<ValidationException> ValidateAllScenes()
        {
            List<ValidationException> exceptions = new List<ValidationException>();

            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                foreach (GameObject rootObj in scene.GetRootGameObjects())
                {
                    exceptions.AddRange(RequiredFieldValidator.ValidateGameObject(rootObj, scenePath));
                }
            }

            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }

            return exceptions;
        }

        #endregion
    }
}
