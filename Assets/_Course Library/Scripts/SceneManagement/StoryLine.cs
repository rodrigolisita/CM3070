using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Holds all data for a single line of dialogue in a story sequence.
/// This class is used within a StorySequenceAsset.
/// </summary>
[System.Serializable]
public class StoryLine
{
    [Tooltip("The localized text content for this line.")]
    public LocalizedString localizedText;

    [Tooltip("The localized narration audio clip that corresponds to the text.")]
    public LocalizedAsset<AudioClip> localizedAudio;

    [Tooltip("How fast the text appears, in characters per second.")]
    public float charsPerSecond = 25f; // <<< ADD THIS LINE

    [Tooltip("The pause in seconds after this line has finished displaying and narrating.")]
    public float postLineDelay = 1.0f;
}