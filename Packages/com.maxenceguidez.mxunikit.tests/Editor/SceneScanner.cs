using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MxUnikit.Tests.Editor
{
    public static class SceneScanner
    {
        public static List<ValidationException> ValidateAllScenes()
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
    }
}
