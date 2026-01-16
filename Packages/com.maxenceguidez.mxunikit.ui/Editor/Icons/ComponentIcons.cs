using UnityEditor;
using UnityEngine;

namespace MxUnikit.UI.Editor.Icons
{
    [InitializeOnLoad]
    public static class ComponentIcons
    {
        // ----------------------------------------------------------------------------------------
        static ComponentIcons()
        {
            MonoImporter monoImporter = AssetImporter.GetAtPath("Packages/com.maxenceguidez.mxunikit.ui/Runtime/MxText.cs") as MonoImporter;
            MonoScript monoScript = monoImporter.GetScript();
            Texture2D i18nIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.maxenceguidez.mxunikit.ui/Editor/Icons/MxText.png");

            EditorGUIUtility.SetIconForObject(monoScript, i18nIcon);
        }

        // ----------------------------------------------------------------------------------------
    }
}