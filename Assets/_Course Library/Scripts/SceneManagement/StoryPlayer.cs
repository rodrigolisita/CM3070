using UnityEngine;
using System.Collections;

/// <summary>
/// Orchestrates the playback of a StorySequenceAsset by coordinating the UI view and narration player.
/// Manages the overall flow, timing, and skip logic for the sequence.
/// </summary>
public class StoryPlayer : MonoBehaviour
{
    [Header("Data Asset")]
    [Tooltip("The story sequence asset to play.")]
    [SerializeField] private StorySequenceAsset storyToPlay;

    [Header("System References")]
    [Tooltip("The component responsible for displaying text elements in the UI.")]
    [SerializeField] private UIStoryView uiView;
    [Tooltip("The component responsible for playing narration audio clips.")]
    [SerializeField] private NarrationPlayer narrationPlayer;

    private int currentLineIndex = 0;
    private bool isLinePlaying = false;

    private Coroutine storyCoroutine;
    private bool isStoryActive = false;

    void Awake()
    {
        // Auto-find components if they haven't been assigned in the inspector.
        // This assumes UIStoryView is on the same GameObject as StoryPlayer.
        if (uiView == null)
        {
            uiView = GetComponent<UIStoryView>();
            if (uiView == null)
            {
                Debug.LogError("StoryPlayer needs a UIStoryView component on the same GameObject.", this);
            }
        }

        // This assumes NarrationPlayer is on a child GameObject.
        if (narrationPlayer == null)
        {
            narrationPlayer = GetComponentInChildren<NarrationPlayer>();
            if (narrationPlayer == null)
            {
                Debug.LogError("StoryPlayer could not find a NarrationPlayer component in its children.", this);
            }
        }
    }    

    void Start()
    {
        if (storyToPlay == null || uiView == null || narrationPlayer == null)
        {
            Debug.LogError("StoryPlayer references are incomplete. Please assign all fields.");
            return;
        }
        isStoryActive = true; // Set flag to true when starting
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Iterate through every line in the story asset
        for (currentLineIndex = 0; currentLineIndex < storyToPlay.lines.Count; currentLineIndex++)
        {
            // Safety check: If story was stopped while waiting for the previous line, exit now.
            if (!isStoryActive) yield break;

            isLinePlaying = true;
            StoryLine currentLine = storyToPlay.lines[currentLineIndex];

            // 1. Start both text display and audio playback simultaneously
            Coroutine textCoroutine = StartCoroutine(uiView.DisplayTextLine(currentLine));
            Coroutine audioCoroutine = StartCoroutine(narrationPlayer.PlayNarration(currentLine.localizedAudio));

            // 2. Wait for both text animation and audio clip to finish before proceeding
            yield return textCoroutine;
            yield return audioCoroutine;

            // 3. Post-line delay
            isLinePlaying = false;
            yield return new WaitForSeconds(currentLine.postLineDelay);
        }
        Debug.Log("Story sequence finished.");
        isStoryActive = false;
    }

    /// <summary>
    /// Call this function from a button press to skip the current line's animation and audio.
    /// </summary>
    public void SkipCurrentLine()
    {
        if (isLinePlaying)
        {
            uiView.SkipTypewriter();
            narrationPlayer.StopAudio();
        }
    }

     /// <summary>
    /// Completely stops the story sequence and any playing audio.
    /// Call this when exiting the story view.
    /// </summary>
    public void StopStorySequence()
    {
        // 1. Set flag immediately to prevent loop continuation.
        isStoryActive = false;

        // 2. Stop audio player.
        if (narrationPlayer != null)
        {
            narrationPlayer.StopAudio();
        }

        StopAllCoroutines();
        
        // Stop the main playback loop
        //if (storyCoroutine != null)
        //{
        //    StopCoroutine(storyCoroutine);
        //}

        // Ensure audio stops immediately
        //if (narrationPlayer != null)
        //{
        //    narrationPlayer.StopAudio();
        //}

        // Optional: Disable the StorySystem object itself if no longer needed
        // gameObject.SetActive(false);
    }
}