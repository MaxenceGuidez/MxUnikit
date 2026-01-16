using UnityEngine;

namespace MxUnikit.I18n
{
    public interface MxI18nText
    {
        // --------------------------------------------------------------------------------------------
        public string GetI18nText(string baseText);

        // --------------------------------------------------------------------------------------------
        public void RefreshI18nText();

        // --------------------------------------------------------------------------------------------
        public static void RefreshAll()
        {
            MonoBehaviour[] allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (MonoBehaviour behaviour in allBehaviours)
            {
                if (behaviour is MxI18nText text)
                {
                    text.RefreshI18nText();
                }
            }
        }

        // --------------------------------------------------------------------------------------------
    }
}