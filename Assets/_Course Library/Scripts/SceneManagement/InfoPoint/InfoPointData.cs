using UnityEngine;

[CreateAssetMenu(fileName = "NewInfoPointData", menuName = "Narrative/Info Point Data")]
public class InfoPointData : ScriptableObject
{
    [Header("Content")]
    [Tooltip("The Narrative Segment that this info point will display.")]
    public NarrativeSegment narrativeSegment;

    [Header("Presentation")]
    [Tooltip("The UI Prefab (containing the Canvas, buttons, etc.) to instantiate for this story.")]
    public GameObject interfacePrefab;

    [Header("Settings")]
    [Tooltip("If checked, this info point will start its narrative as soon as the scene loads.")]
    public bool startAutomatically = false;
}