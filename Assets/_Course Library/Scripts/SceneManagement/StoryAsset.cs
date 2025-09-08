using UnityEngine;
using System.Collections.Generic;

// This attribute allows you to create instances from the Project menu:
// Right-click > Create > Storytelling > New Story Segment
[CreateAssetMenu(fileName = "NewStorySegment", menuName = "Storytelling/New Story Segment")]
public class StoryAsset : ScriptableObject
{
    public string storyTitle;
    public List<DialogueLine> dialogueLines;
    // You could also add overall story properties here, like background music for the segment.
}