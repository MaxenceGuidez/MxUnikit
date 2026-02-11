using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MxUnikit.I18n
{
    /// <summary>
    /// Static class for handling internationalization (I18n) in Unity projects.
    /// Manages loading language files and translating text keys.
    /// </summary>
    public static class MxI18n
    {
        // ----------------------------------------------------------------------------------------
        private static Dictionary<string, string> currentLanguageI18n = new Dictionary<string, string>();
        private static List<SystemLanguage> supportedLanguages;

        private const string I18nResourcesPath = "I18n/";

        // ----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the current language being used.
        /// </summary>
        public static SystemLanguage CurrentLanguage { get; private set; } = SystemLanguage.English;

        /// <summary>
        /// Gets the list of supported languages based on available resources.
        /// </summary>
        public static List<SystemLanguage> SupportedLanguages
        {
            get
            {
                if (supportedLanguages != null) return supportedLanguages;

                supportedLanguages = new List<SystemLanguage>();

                for (SystemLanguage systemLanguage = 0; systemLanguage < SystemLanguage.Unknown; systemLanguage++)
                {
                    TextAsset i18nFile = Resources.Load<TextAsset>(I18nResourcesPath + systemLanguage);
                    if (i18nFile == null) continue;

                    supportedLanguages.Add(systemLanguage);
                }

                return supportedLanguages;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the current language and reloads all translations.
        /// </summary>
        /// <param name="newLanguage">The new language to set; if Unknown, uses system language.</param>
        public static void SetCurrentLanguage(SystemLanguage newLanguage = SystemLanguage.Unknown)
        {
            if (newLanguage == SystemLanguage.Unknown)
            {
                newLanguage = Application.systemLanguage;
            }

            if (!SupportedLanguages.Contains(newLanguage))
            {
                Debug.LogWarning($"Language {newLanguage} is not supported. Fallback to English.");
                newLanguage = SystemLanguage.English;
            }

            currentLanguageI18n.Clear();
            LoadLanguage(newLanguage);

            CurrentLanguage = newLanguage;

            MxI18nText.RefreshAll();
        }

        // ----------------------------------------------------------------------------------------
        /// <summary>
        /// Translates a key to the current language, with optional string replacements.
        /// </summary>
        /// <param name="key">The translation key.</param>
        /// <param name="values">Values to replace placeholders (%s) in the translation.</param>
        /// <returns>The translated string.</returns>
        public static string T(string key, params string[] values)
        {
            if (string.IsNullOrEmpty(key)) return "";

            if (currentLanguageI18n.TryGetValue(key, out string text))
            {
                foreach (string value in values)
                {
                    int index = text.IndexOf("%s", StringComparison.Ordinal);
                    if (index == -1) break;

                    text = text.Remove(index, 2).Insert(index, value ?? "");
                }

                return text;
            }

            // Fallback to the key itself if not found
            string fallback = key;

            foreach (string value in values)
            {
                int index = fallback.IndexOf("%s", StringComparison.Ordinal);
                if (index == -1) break;

                fallback = fallback.Remove(index, 2).Insert(index, value ?? "");
            }

            return fallback;
        }

        // ----------------------------------------------------------------------------------------
        /// <summary>
        /// Loads translations from the resource file of the specified language.
        /// </summary>
        /// <param name="language">Language to load.</param>
        private static void LoadLanguage(SystemLanguage language)
        {
            TextAsset i18nFile = Resources.Load<TextAsset>(I18nResourcesPath + language);
            if (i18nFile == null)
            {
                Debug.LogError($"I18n file for language {language} does not exist.");
                return;
            }

            using StringReader sr = new StringReader(i18nFile.text);
            while (sr.ReadLine() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line) || line[0] == '#') continue;

                string[] parts = line.Split('=', 2);
                if (parts.Length < 2 || string.IsNullOrEmpty(parts[1])) continue;

                currentLanguageI18n[parts[0]] = parts[1].Replace("<br>", "\n").Replace("\\n", "\n");
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}
