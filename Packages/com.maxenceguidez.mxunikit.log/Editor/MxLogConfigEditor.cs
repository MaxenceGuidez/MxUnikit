using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MxUnikit.Log.Editor
{
    [CustomEditor(typeof(MxLogConfig))]
    public class MxLogConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _isEnabled;
        private SerializedProperty _logStackTraceForExceptions;
        private SerializedProperty _categories;

        private ReorderableList _categoriesList;

        private bool _showGeneralSettings = true;
        private bool _showCategories = true;

        #region Setup

        private void OnEnable()
        {
            _isEnabled = serializedObject.FindProperty("IsEnabled");
            _logStackTraceForExceptions = serializedObject.FindProperty("LogStackTraceForExceptions");
            _categories = serializedObject.FindProperty("Categories");

            SetupCategoriesList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGeneralSettings();

            EditorGUILayout.Space(8);

            DrawCategories();

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupCategoriesList()
        {
            _categoriesList = new ReorderableList(serializedObject, _categories, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    float idWidth = rect.width * 0.18f;
                    float colorWidth = rect.width * 0.18f;
                    float enabledWidth = rect.width * 0.12f;
                    float keywordsWidth = rect.width * 0.40f;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, idWidth, rect.height), "Category ID", EditorStyles.boldLabel);
                    EditorGUI.LabelField(new Rect(rect.x + idWidth, rect.y, colorWidth, rect.height), "Color", EditorStyles.boldLabel);
                    EditorGUI.LabelField(new Rect(rect.x + idWidth + colorWidth, rect.y, enabledWidth, rect.height), "Enabled", EditorStyles.boldLabel);
                    EditorGUI.LabelField(new Rect(rect.x + idWidth + colorWidth + enabledWidth, rect.y, keywordsWidth, rect.height), "Keywords", EditorStyles.boldLabel);
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SerializedProperty element = _categories.GetArrayElementAtIndex(index);
                    SerializedProperty categoryId = element.FindPropertyRelative("CategoryId");
                    SerializedProperty color = element.FindPropertyRelative("Color");
                    SerializedProperty isEnabled = element.FindPropertyRelative("IsEnabled");
                    SerializedProperty keywords = element.FindPropertyRelative("Keywords");

                    rect.y += 2;
                    float idWidth = rect.width * 0.18f;
                    float colorWidth = rect.width * 0.18f;
                    float enabledWidth = rect.width * 0.12f;
                    float keywordsWidth = rect.width * 0.40f;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, idWidth - 5, EditorGUIUtility.singleLineHeight),
                        categoryId,
                        GUIContent.none
                    );

                    string colorHex = color.stringValue;
                    Color currentColor = Color.white;
                    if (ColorUtility.TryParseHtmlString(colorHex, out Color parsedColor))
                    {
                        currentColor = parsedColor;
                    }

                    EditorGUI.BeginChangeCheck();
                    Color newColor = EditorGUI.ColorField(
                        new Rect(rect.x + idWidth, rect.y, colorWidth - 5, EditorGUIUtility.singleLineHeight),
                        GUIContent.none,
                        currentColor,
                        true,
                        true,
                        false
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        color.stringValue = "#" + ColorUtility.ToHtmlStringRGB(newColor);
                    }

                    EditorGUI.PropertyField(
                        new Rect(rect.x + idWidth + colorWidth, rect.y, enabledWidth - 5, EditorGUIUtility.singleLineHeight),
                        isEnabled,
                        GUIContent.none
                    );

                    EditorGUI.PropertyField(
                        new Rect(rect.x + idWidth + colorWidth + enabledWidth, rect.y, keywordsWidth - 5, EditorGUIUtility.singleLineHeight),
                        keywords,
                        GUIContent.none
                    );
                },
                elementHeightCallback = index =>
                {
                    SerializedProperty element = _categories.GetArrayElementAtIndex(index);
                    SerializedProperty keywords = element.FindPropertyRelative("Keywords");

                    float baseHeight = EditorGUIUtility.singleLineHeight + 4;

                    if (!keywords.isExpanded) return baseHeight;

                    int keywordCount = keywords.arraySize;
                    float keywordsHeight = (keywordCount + 2) * (EditorGUIUtility.singleLineHeight + 2);

                    return baseHeight + keywordsHeight;
                }
            };
        }

        #endregion

        #region Draws

        private void DrawGeneralSettings()
        {
            _showGeneralSettings = DrawFoldoutSection("General Settings", _showGeneralSettings);
            if (!_showGeneralSettings) return;

            EditorGUILayout.BeginVertical(GetSectionBoxStyle());
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_isEnabled, new GUIContent("Enable Logging", "Master switch to enable or disable all logging"));
            EditorGUILayout.PropertyField(_logStackTraceForExceptions, new GUIContent("Log Stack Trace for Exceptions", "Include full stack trace when logging exceptions"));

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawCategories()
        {
            _showCategories = DrawFoldoutSection($"Categories ({_categories.arraySize})", _showCategories);
            if (!_showCategories) return;

            EditorGUILayout.BeginVertical(GetSectionBoxStyle());

            EditorGUILayout.HelpBox(
                "Categories define how logs are organized and displayed. Each category has a unique ID, color, and keywords for auto-detection.\n" +
                "Keywords are used to automatically detect the category based on class name, method name, or message content.",
                MessageType.Info
            );

            EditorGUILayout.Space(5);

            _categoriesList.DoLayoutList();

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Utils

        private static bool DrawFoldoutSection(string title, bool foldout)
        {
            GUIStyle foldoutBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(20, 10, 5, 5)
            };

            EditorGUILayout.BeginVertical(foldoutBoxStyle);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
            };
            bool newFoldout = EditorGUILayout.Foldout(foldout, title, true, foldoutStyle);
            EditorGUILayout.EndVertical();

            return newFoldout;
        }

        private static GUIStyle GetSectionBoxStyle()
        {
            return new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };
        }

        #endregion
    }
}
