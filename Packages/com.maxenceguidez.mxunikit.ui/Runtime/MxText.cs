using MxUnikit.I18n;
using TMPro;
using UnityEngine;

namespace MxUnikit.UI
{
    [AddComponentMenu("UI/MxUnikit/MxText")]
    public class MxText : TextMeshProUGUI, MxI18nText
    {
        #region MxI18nText
        // ----------------------------------------------------------------------------------------
        public string I18nKey;
        public string[] Values = { };
        public bool[] ShouldTranslateValues = { };

        // --------------------------------------------------------------------------------------------
        public new void Start()
        {
            RefreshI18nText();
            base.Start();
        }

        // --------------------------------------------------------------------------------------------
        public string GetI18nText(string baseText)
        {
            if (string.IsNullOrEmpty(I18nKey)) return baseText;

            string[] localizedValues = new string[Values.Length];

            for (int i = 0; i < Values.Length; i++)
            {
                localizedValues[i] = ShouldTranslateValues[i] ? MxI18n.T(Values[i]) : Values[i];
            }

            return MxI18n.T(I18nKey, localizedValues);
        }

        // ----------------------------------------------------------------------------------------
        public void RefreshI18nText()
        {
            text = GetI18nText(text);
        }
        #endregion

        // ----------------------------------------------------------------------------------------
    }
}