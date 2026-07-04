using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MxUnikit.Provider.Editor
{
    public class MxProviderWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Window/MxUnikit/Provider")]
        public static void ShowWindow()
        {
            MxProviderWindow window = GetWindow<MxProviderWindow>("Provider");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("MxProvider - Registered Instances", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            IReadOnlyDictionary<Type, object> instances = MxProvider.GetAll();

            if (instances.Count == 0)
            {
                EditorGUILayout.HelpBox("No instances registered.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Total Instances: {instances.Count}");
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (KeyValuePair<Type, object> kvp in instances)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(kvp.Key.Name, GUILayout.Width(200));
                EditorGUILayout.LabelField(kvp.Value?.GetType().Name ?? "null", GUILayout.Width(200));

                if (kvp.Value is MonoBehaviour mb)
                {
                    EditorGUILayout.ObjectField(mb, typeof(MonoBehaviour), true, GUILayout.Width(150));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Clear All Instances"))
            {
                if (EditorUtility.DisplayDialog("Clear Instances",
                    "Are you sure you want to clear all registered instances?", "Yes", "Cancel"))
                {
                    MxProvider.Clear();
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh"))
            {
                Repaint();
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
