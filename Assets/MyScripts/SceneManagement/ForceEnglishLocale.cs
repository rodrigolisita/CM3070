using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ForceEnglishLocale : MonoBehaviour
{
    void Start()
    {
        var englishLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
        LocalizationSettings.SelectedLocale = englishLocale;
    }
}
