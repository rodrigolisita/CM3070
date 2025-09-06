using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A small helper class to link a Locale to an AudioClip in the Inspector.
/// </summary>
[System.Serializable]
public class LocalizedAudioClip
{
    // The locale identifier (e.g., "en" for English, "pt-BR" for Brazilian Portuguese).
    public LocaleIdentifier locale;
    // The audio clip for this specific locale.
    public AudioClip audioClip;
}

/// <summary>
/// Manages playing a narration audio clip that matches the current selected language.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class NarrationManager : MonoBehaviour
{
    [Header("Narration Settings")]
    [Tooltip("The list of audio clips, each linked to a specific language.")]
    [SerializeField] private List<LocalizedAudioClip> narrations;

    [Tooltip("The delay in seconds before the narration starts playing. Use this to sync with text animations.")]
    [SerializeField] private float narrationStartDelay = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Get the AudioSource component on this GameObject.
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Start the process of playing the narration.
        StartCoroutine(PlayLocalizedNarration());
    }

    private IEnumerator PlayLocalizedNarration()
    {
        // 1. Wait for the initial delay.
        yield return new WaitForSeconds(narrationStartDelay);

        // 2. Get the currently selected locale from Unity's Localization system.
        Locale currentLocale = LocalizationSettings.SelectedLocale;
        AudioClip clipToPlay = null;

        // 3. Find the AudioClip in our list that matches the current locale.
        foreach (var localizedAudio in narrations)
        {
            if (localizedAudio.locale.Equals(currentLocale.Identifier))
            {
                clipToPlay = localizedAudio.audioClip;
                break; // Exit the loop once we've found our match.
            }
        }

        // 4. If we found a matching clip, assign it and play it.
        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
        else
        {
            // If no clip is found for the current language, log an error.
            Debug.LogError($"NarrationManager: No audio clip found for the current locale '{currentLocale.Identifier.Code}'.", this);
        }
    }
}