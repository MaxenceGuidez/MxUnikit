using MxUnikit.Extensions;
using MxUnikit.I18n;
using MxUnikit.Log;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class I18nSelector : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------
    private static bool RefreshRequest;

    // ----------------------------------------------------------------------------------------
    [SerializeField] private Button buttonLangPrefab;

    // ----------------------------------------------------------------------------------------
    private RectTransform contentButtonsLang;

    // ----------------------------------------------------------------------------------------
    private void Awake()
    {
        contentButtonsLang = GetComponent<RectTransform>();
    }

    // ----------------------------------------------------------------------------------------
    private void Start()
    {
        MxLog.L("SetCurrentLanguage");
        MxI18n.SetCurrentLanguage();
        RefreshRequest = true;
    }

    // ----------------------------------------------------------------------------------------
    private void Update()
    {
        if (RefreshRequest)
        {
            RefreshRequest = false;
            Refresh();
        }
    }

    // ----------------------------------------------------------------------------------------
    private void Refresh()
    {
        MxLog.L("Refresh");
        contentButtonsLang.DestroyChildren();

        foreach (SystemLanguage lang in MxI18n.SupportedLanguages)
        {
            InstantiateButtonLang(lang);
        }

        Canvas.ForceUpdateCanvases();
    }

    // ----------------------------------------------------------------------------------------
    private void InstantiateButtonLang(SystemLanguage lang)
    {
        Button button = Instantiate(buttonLangPrefab, contentButtonsLang);
        button.onClick.AddListener(() => OnClickButtonLang(lang));

        bool isSelected = lang == MxI18n.CurrentLanguage;
        MxLog.L($"Button {lang} is selected: {isSelected}");

        ColorBlock block = button.colors;
        block.normalColor = isSelected ? Color.coral : Color.white;
        button.colors = block;

        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = lang.ToString();
    }

    // ----------------------------------------------------------------------------------------
    private void OnClickButtonLang(SystemLanguage lang)
    {
        MxLog.L($"SetCurrentLanguage: {lang}");
        MxI18n.SetCurrentLanguage(lang);
        RefreshRequest = true;
    }

    // ----------------------------------------------------------------------------------------
}
