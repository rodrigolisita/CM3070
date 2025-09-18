using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// A generic script to populate a TMP_Dropdown with a list of LocalizedStrings.
/// </summary>
public class DropdownPopulator : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("The dropdown UI element to populate.")]
    public TMP_Dropdown dropdown;

    [Header("Dropdown Content")]
    [Tooltip("The list of localized string keys to use as options.")]
    public List<LocalizedString> localizedOptions = new List<LocalizedString>();

    void Start()
    {
        // Ensure the dropdown reference is set
        if (dropdown == null)
        {
            dropdown = GetComponent<TMP_Dropdown>();
            if(dropdown == null)
            {
                Debug.LogError("Dropdown reference not set on DropdownPopulator!", this);
                return;
            }
        }

        // Subscribe to the language change event to repopulate the dropdown
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        // Populate the dropdown with the initial language
        Populate();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to prevent errors
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    /// <summary>
    /// Called when the system's locale (language) is changed.
    /// </summary>
    private void OnLocaleChanged(Locale obj)
    {
        Populate();
    }

    /// <summary>
    /// Clears and repopulates the dropdown with the translated strings.
    /// </summary>
    private void Populate()
    {
        dropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var localizedString in localizedOptions)
        {
            // Get the translated string from the localization table
            options.Add(localizedString.GetLocalizedString());
        }

        dropdown.AddOptions(options);
    }
}