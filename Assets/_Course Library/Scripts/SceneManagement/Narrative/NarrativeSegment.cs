using UnityEngine;
using UnityEngine.SceneManagement;

// Possible actions
public enum PostSegmentAction
{
    DoNothing,
    LoadScene
}

/// <summary>
/// A ScriptableObject that defines a complete narrative unit.
/// It acts as a master package, linking to the story content and the UI blueprint.
/// </summary>
[CreateAssetMenu(fileName = "NewNarrativeSegment", menuName = "Narrative/New Narrative Segment")]
public class NarrativeSegment : ScriptableObject
{
    [Header("Content")]
    [Tooltip("The dialogue sequence (text, audio, portraits) to be played for this segment.")]
    public StorySequenceAsset dialogue;

    [Header("Presentation")]
    [Tooltip("The UI blueprint that defines how the story system will look and feel.")]
    public StoryUILayout uiLayout;

    // You can add more high-level data here later, like objectives or rewards.

    [Header("Post-Segment Action")]
    [Tooltip("What should happen after this narrative segment is finished?")]
    public PostSegmentAction actionOnFinish = PostSegmentAction.DoNothing;

    [Tooltip("The delay in seconds before the action is executed.")]
    public float delayBeforeAction = 2.0f;

    [Tooltip("The name of the scene to load. Only used if 'Action On Finish' is set to LoadScene.")]
    public string sceneToLoad;
}