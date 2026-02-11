using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MxUnikit.ServiceLocator.Editor
{
	public class MxServiceLocatorWindow : EditorWindow
	{
		private Vector2 _scrollPosition;

		[MenuItem("Window/MxUnikit/Service Locator")]
		public static void ShowWindow()
		{
			MxServiceLocatorWindow window = GetWindow<MxServiceLocatorWindow>("Service Locator");
			window.Show();
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("MxServiceLocator - Registered Services", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			IReadOnlyDictionary<Type, IService> services = MxServiceLocator.GetAllServices();

			if (services.Count == 0)
			{
				EditorGUILayout.HelpBox("No services registered.", MessageType.Info);
				return;
			}

			EditorGUILayout.LabelField($"Total Services: {services.Count}");
			EditorGUILayout.Space();

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			foreach (KeyValuePair<Type, IService> kvp in services)
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

			if (GUILayout.Button("Clear All Services"))
			{
				if (EditorUtility.DisplayDialog("Clear Services",
					"Are you sure you want to clear all registered services?", "Yes", "Cancel"))
				{
					MxServiceLocator.Clear();
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
