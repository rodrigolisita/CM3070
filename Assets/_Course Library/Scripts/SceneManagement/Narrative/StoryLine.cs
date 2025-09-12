using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

/// <summary>
/// Holds all data for a single line of dialogue in a story sequence.
/// This class is used within a StorySequenceAsset.
/// </summary>
[System.Serializable]
public class StoryLine
{
    [Tooltip("The portrait image to display for this speaker.")]
    public Sprite characterPortrait;

    [Tooltip("The localized text content for this line.")]
    public LocalizedString localizedText;

    [Tooltip("The localized narration audio clip that corresponds to the text.")]
    public LocalizedAsset<AudioClip> localizedAudio;

    [Tooltip("How fast the text appears, in characters per second.")]
    public float charsPerSecond = 25f; 

    [Tooltip("The pause in seconds after this line has finished displaying and narrating.")]
    public float postLineDelay = 1.0f;

    [Header("Branching Options")]
    [Tooltip("Add choices here to create a branch. If this list is empty, the story continues to the next line automatically.")]
    public List<ChoiceData> choices;
}