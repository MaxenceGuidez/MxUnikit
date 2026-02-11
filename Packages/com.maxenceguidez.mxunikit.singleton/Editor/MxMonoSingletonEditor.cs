using UnityEditor;

namespace MxUnikit.Singleton.Editor
{
	[CustomEditor(typeof(MxMonoSingleton<>), true)]
	public class MxMonoSingletonEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(
				"This is a Singleton MonoBehaviour. Only one instance can exist at a time.\n" +
				"Check Mode settings in derived class to control behavior.",
				MessageType.Info
			);

			EditorGUILayout.Space();

			DrawDefaultInspector();
		}
	}
}
