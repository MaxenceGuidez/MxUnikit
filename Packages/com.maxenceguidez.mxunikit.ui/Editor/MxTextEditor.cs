using MxUnikit.I18n;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace MxUnikit.UI.Editor
{
    [CustomEditor(typeof(MxText))]
    public class MxTextEditor : TMP_EditorPanelUI
    {
        private bool showValuesSection;
        private SerializedProperty valuesBoolProp;
        private SerializedProperty valuesProp;
        private int selectedTab;

        // ----------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();
            valuesProp = serializedObject.FindProperty("Values");
            valuesBoolProp = serializedObject.FindProperty("ShouldTranslateValues");
        }

        // ----------------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIContent[] tabContents =
            {
                new GUIContent("TMP", AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.ugui/Editor Resources/Gizmos/TMP - Text Component Icon.psd")),
                new GUIContent("MxText", AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.maxenceguidez.mxunikit.ui/Editor/Icons/MxText.png"))
            };
            selectedTab = GUILayout.Toolbar(selectedTab, tabContents, GUILayout.Height(30));

            EditorGUILayout.Space();

            if (selectedTab == 0)
            {
                base.OnInspectorGUI();
            }
            else
            {
                DrawI18nInspector();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // ----------------------------------------------------------------------------------------
        private void DrawI18nInspector()
        {
            DrawKeyInput();
            DrawLanguageButtons();

            showValuesSection = EditorGUILayout.Foldout(showValuesSection, "Values", true);
            if (showValuesSection)
            {
                DrawValues();
            }
        }

        // ----------------------------------------------------------------------------------------
        private void DrawKeyInput()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("I18nKey"));
        }

        // ----------------------------------------------------------------------------------------
        private void DrawLanguageButtons()
        {
            int currentIndex = System.Array.IndexOf(MxI18n.SupportedLanguages.ToArray(), MxI18n.CurrentLanguage);

            string[] languageNames = new string[MxI18n.SupportedLanguages.Count];
            for (int i = 0; i < MxI18n.SupportedLanguages.Count; i++)
            {
                languageNames[i] = MxI18n.SupportedLanguages[i].ToString();
            }

            int selectedIndex = GUILayout.Toolbar(currentIndex, languageNames);
            if (selectedIndex == currentIndex) return;

            MxI18n.SetCurrentLanguage(MxI18n.SupportedLanguages[selectedIndex]);
            RefreshText();
        }

        // ----------------------------------------------------------------------------------------
        private void DrawValues()
        {
            SyncArraySizes();

            EditorGUI.indentLevel++;

            for (int i = 0; i < valuesProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Value field
                EditorGUILayout.PropertyField(valuesProp.GetArrayElementAtIndex(i), GUIContent.none);

                // ShouldTranslate checkbox 
                valuesBoolProp.GetArrayElementAtIndex(i).boolValue = GUILayout.Toggle(valuesBoolProp.GetArrayElementAtIndex(i).boolValue, "Should Translate Value");

                // DeleteValue button
                GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 20,
                    fixedHeight = 18
                };

                if (GUILayout.Button("✕", deleteButtonStyle))
                {
                    valuesProp.DeleteArrayElementAtIndex(i);
                    valuesBoolProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.Space(4);

            // AddValue button
            GUIStyle addButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 22
            };

            if (GUILayout.Button("+ Add Value", addButtonStyle))
            {
                valuesProp.InsertArrayElementAtIndex(valuesProp.arraySize);
                valuesBoolProp.InsertArrayElementAtIndex(valuesBoolProp.arraySize);
                valuesBoolProp.GetArrayElementAtIndex(valuesBoolProp.arraySize - 1).boolValue = false;
            }

            EditorGUI.indentLevel--;
        }


        // ----------------------------------------------------------------------------------------
        private void SyncArraySizes()
        {
            while (valuesBoolProp.arraySize < valuesProp.arraySize)
            {
                valuesBoolProp.InsertArrayElementAtIndex(valuesBoolProp.arraySize);
                valuesBoolProp.GetArrayElementAtIndex(valuesBoolProp.arraySize - 1).boolValue = false;
            }

            while (valuesBoolProp.arraySize > valuesProp.arraySize) valuesBoolProp.DeleteArrayElementAtIndex(valuesBoolProp.arraySize - 1);
        }

        // ----------------------------------------------------------------------------------------
        private void RefreshText()
        {
            MxText textComponent = target as MxText;
            if (textComponent == null) return;

            textComponent.RefreshI18nText();

            EditorUtility.SetDirty(textComponent);
        }

        // ----------------------------------------------------------------------------------------
    }
}