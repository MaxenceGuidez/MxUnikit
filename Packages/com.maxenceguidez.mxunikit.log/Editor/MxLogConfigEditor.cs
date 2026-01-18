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
        private SerializedProperty _disabledCategories;
        private SerializedProperty _categoryColors;
        private SerializedProperty _categoryKeywords;

        private ReorderableList _disabledCategoriesList;
        private ReorderableList _categoryColorsList;
        private ReorderableList _categoryKeywordsList;

        private bool _showGeneralSettings;
        private bool _showDisabledCategories;
        private bool _showCategoryColors;
        private bool _showCategoryKeywords;

        private void OnEnable()
        {
            _isEnabled = serializedObject.FindProperty("IsEnabled");
            _logStackTraceForExceptions = serializedObject.FindProperty("LogStackTraceForExceptions");
            _disabledCategories = serializedObject.FindProperty("DisabledCategories");
            _categoryColors = serializedObject.FindProperty("CategoryColors");
            _categoryKeywords = serializedObject.FindProperty("CategoryKeywords");

            SetupDisabledCategoriesList();
            SetupCategoryColorsList();
            SetupCategoryKeywordsList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGeneralSettings();
            EditorGUILayout.Space(5);

            DrawDisabledCategories();
            EditorGUILayout.Space(5);

            DrawCategoryColors();
            EditorGUILayout.Space(5);

            DrawCategoryKeywords();
            EditorGUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        #region Setups

        private void SetupDisabledCategoriesList()
        {
            _disabledCategoriesList = new ReorderableList(serializedObject, _disabledCategories, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Categories to Disable", EditorStyles.boldLabel);
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SerializedProperty element = _disabledCategories.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    EditorGUI.PropertyField(rect, element, GUIContent.none);
                },
                elementHeight = EditorGUIUtility.singleLineHeight + 4
            };
        }

        private void SetupCategoryColorsList()
        {
            _categoryColorsList = new ReorderableList(serializedObject, _categoryColors, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    float categoryWidth = rect.width * 0.5f;
                    float colorWidth = rect.width * 0.5f;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, categoryWidth, rect.height), "Category", EditorStyles.boldLabel);
                    EditorGUI.LabelField(new Rect(rect.x + categoryWidth, rect.y, colorWidth, rect.height), "Color", EditorStyles.boldLabel);
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SerializedProperty element = _categoryColors.GetArrayElementAtIndex(index);
                    SerializedProperty categoryProp = element.FindPropertyRelative("Category");
                    SerializedProperty colorProp = element.FindPropertyRelative("Color");

                    rect.y += 2;
                    float categoryWidth = rect.width * 0.5f;
                    float colorWidth = rect.width * 0.45f;
                    float spacing = rect.width * 0.05f;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, categoryWidth - 5, EditorGUIUtility.singleLineHeight),
                        categoryProp,
                        GUIContent.none
                    );

                    string colorHex = colorProp.stringValue;
                    Color currentColor = Color.white;
                    if (ColorUtility.TryParseHtmlString(colorHex, out Color parsedColor))
                    {
                        currentColor = parsedColor;
                    }

                    EditorGUI.BeginChangeCheck();
                    Color newColor = EditorGUI.ColorField(
                        new Rect(rect.x + categoryWidth + spacing, rect.y, colorWidth, EditorGUIUtility.singleLineHeight),
                        GUIContent.none,
                        currentColor,
                        true,
                        true,
                        false
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        colorProp.stringValue = "#" + ColorUtility.ToHtmlStringRGB(newColor);
                    }
                },
                elementHeight = EditorGUIUtility.singleLineHeight + 4
            };
        }

        private void SetupCategoryKeywordsList()
        {
            _categoryKeywordsList = new ReorderableList(serializedObject, _categoryKeywords, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    float keywordWidth = rect.width * 0.5f;
                    float categoryWidth = rect.width * 0.5f;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, keywordWidth, rect.height), "Keyword", EditorStyles.boldLabel);
                    EditorGUI.LabelField(new Rect(rect.x + keywordWidth, rect.y, categoryWidth, rect.height), "Category", EditorStyles.boldLabel);
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    SerializedProperty element = _categoryKeywords.GetArrayElementAtIndex(index);
                    SerializedProperty categoryProp = element.FindPropertyRelative("Category");
                    SerializedProperty keywordProp = element.FindPropertyRelative("Keyword");

                    rect.y += 2;
                    float keywordWidth = rect.width * 0.5f;
                    float categoryWidth = rect.width * 0.45f;
                    float spacing = rect.width * 0.05f;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, keywordWidth - 5, EditorGUIUtility.singleLineHeight),
                        keywordProp,
                        GUIContent.none
                    );

                    EditorGUI.PropertyField(
                        new Rect(rect.x + keywordWidth + spacing, rect.y, categoryWidth, EditorGUIUtility.singleLineHeight),
                        categoryProp,
                        GUIContent.none
                    );
                },
                elementHeight = EditorGUIUtility.singleLineHeight + 4
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

        private void DrawDisabledCategories()
        {
            _showDisabledCategories = DrawFoldoutSection($"Disabled Categories ({_disabledCategories.arraySize})", _showDisabledCategories);
            if (!_showDisabledCategories) return;

            EditorGUILayout.BeginVertical(GetSectionBoxStyle());
            _disabledCategoriesList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryColors()
        {
            _showCategoryColors = DrawFoldoutSection($"Category Colors ({_categoryColors.arraySize})", _showCategoryColors);
            if (!_showCategoryColors) return;

            EditorGUILayout.BeginVertical(GetSectionBoxStyle());
            _categoryColorsList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryKeywords()
        {
            _showCategoryKeywords = DrawFoldoutSection($"Category Keywords ({_categoryKeywords.arraySize})", _showCategoryKeywords);
            if (!_showCategoryKeywords) return;

            EditorGUILayout.BeginVertical(GetSectionBoxStyle());

            EditorGUILayout.HelpBox("Keywords are used to automatically detect the category based on class name, method name, or message content.", MessageType.Info);
            EditorGUILayout.Space(5);

            _categoryKeywordsList.DoLayoutList();
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
