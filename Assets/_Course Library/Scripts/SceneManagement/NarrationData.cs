using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NarrationData", menuName = "Narration/NarrationData")]
public class NarrationData : ScriptableObject
{
    [Header("Text Paragraphs")]
    public List<StoryParagraph> paragraphs;

    [Header("Localized Audio")]
    public List<LocalizedAudioClip> localizedNarrations;

    [Header("Narration Timing")]
    public float narrationStartDelay = 0.5f;
}