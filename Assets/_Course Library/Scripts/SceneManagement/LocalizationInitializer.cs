using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LocalizationInitializer : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(InitializeLocalization());
    }

    IEnumerator InitializeLocalization()
    {
        // Wait until the localization system has finished its initialization operation.
        yield return LocalizationSettings.InitializationOperation;

        // Now it is safe to access localized strings.
        Debug.Log("Localization Initialized Successfully.");
        // If your welcome message object starts disabled, you can enable it here.
        // welcomeMessageObject.SetActive(true);
    }
}