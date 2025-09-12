using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStorySequence", menuName = "Storytelling/New Story Sequence")]
public class StorySequenceAsset : ScriptableObject
{
    [Header("Sequence Content")]
    [Tooltip("The list of dialogue lines that will be played in order.")]
    public List<StoryLine> lines;

    // Future potential additions: background music for the scene, character portraits, etc.
}