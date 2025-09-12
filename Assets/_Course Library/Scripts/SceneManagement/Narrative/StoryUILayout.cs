using UnityEngine;

/// <summary>
/// A ScriptableObject that acts as a blueprint for a story's UI.
/// It holds references to all the prefabs needed to construct the story view at runtime.
/// </summary>
[CreateAssetMenu(fileName = "NewUILayout", menuName = "Narrative/New UI Layout")]
public class StoryUILayout : ScriptableObject
{
    [Header("Core UI Prefabs")]
    [Tooltip("The prefab for a single text entry, containing TextMeshPro and other components.")]
    public GameObject textEntryPrefab;

    [Tooltip("A prefab of a complete ScrollView, which includes the Content and Viewport children.")]
    public GameObject scrollViewPrefab;

    [Tooltip("A prefab for a single choice button, used for branching narratives.")]
    public GameObject choiceButtonPrefab;

    // Could add more settings here in the future, like fonts, colors, or background images.
}