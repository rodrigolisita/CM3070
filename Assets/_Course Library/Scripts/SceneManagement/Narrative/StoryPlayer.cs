using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Orchestrates the playback of a StorySequenceAsset by coordinating the UI view and narration player.
/// Manages the overall flow, timing, and skip logic for the sequence.
/// </summary>
public class StoryPlayer : MonoBehaviour
{
    public event Action OnSequenceFinished;

    // References will be injected by the GameManager
    private UIStoryView uiView;
    private NarrationPlayer narrationPlayer;
    private StorySequenceAsset storyToPlay;

    private bool isStoryActive = false;
    private int currentLineIndex = 0;

    void Awake()
    {
        // Find the components within the same prefab
        uiView = GetComponentInChildren<UIStoryView>(true);
        narrationPlayer = GetComponentInChildren<NarrationPlayer>(true);
    }

    /// <summary>
    /// Injects the necessary component references from the GameManager.
    /// </summary>
    public void InjectReferences(UIStoryView view, NarrationPlayer narrator)
    {
        this.uiView = view;
        this.narrationPlayer = narrator;
    }

    /// <summary>
    /// Starts playback of the given story sequence. Called by the GameManager.
    /// </summary>
    public void PlayStory(StorySequenceAsset story)
    {
        this.storyToPlay = story;
        isStoryActive = true;
        StartCoroutine(PlaySequence());
    }

    //private IEnumerator PlaySequence()
    //{
    //    for (currentLineIndex = 0; currentLineIndex < storyToPlay.lines.Count; currentLineIndex++)
    //    {
    //        if (!isStoryActive) yield break;

    //        StoryLine currentLine = storyToPlay.lines[currentLineIndex];
    
    //        Coroutine textCoroutine = StartCoroutine(uiView.DisplayTextLine(currentLine));
    //        Coroutine audioCoroutine = StartCoroutine(narrationPlayer.PlayNarration(currentLine.localizedAudio));

    //        yield return textCoroutine;
    //        yield return audioCoroutine;

    //        yield return new WaitForSeconds(currentLine.postLineDelay);
    //    }

    //    isStoryActive = false;
    //    Debug.Log("Story sequence finished.");
    //    OnSequenceFinished?.Invoke(); // Invoke the event here
    //}                
    private IEnumerator PlaySequence()
    {
        // Use a while loop to allow for non-linear jumps
        while (isStoryActive && currentLineIndex < storyToPlay.lines.Count)
        {
            StoryLine currentLine = storyToPlay.lines[currentLineIndex];

            // 1. Display the current dialogue line (text, audio, portrait)
            Coroutine textCoroutine = StartCoroutine(uiView.DisplayTextLine(currentLine));
            Coroutine audioCoroutine = StartCoroutine(narrationPlayer.PlayNarration(currentLine.localizedAudio));
            yield return textCoroutine;
            yield return audioCoroutine;

            // 2. Check for branching choices
            if (currentLine.choices != null && currentLine.choices.Count > 0)
            {
                // Show choices and wait for the user to make a selection.
                // We pass the "OnChoiceSelected" function as a callback.
                yield return StartCoroutine(uiView.DisplayChoices(currentLine.choices, OnChoiceSelected));
                // The loop will now continue from the new index set by the callback.
            }
            else
            {
                // No choices, proceed linearly after the delay
                yield return new WaitForSeconds(currentLine.postLineDelay);
                currentLineIndex++;
            }
        }
        
        isStoryActive = false;
        Debug.Log("Story sequence finished.");
        OnSequenceFinished?.Invoke();
    }

    /// <summary>
    /// This function is passed to the UI to be called when a choice button is clicked.
    /// </summary>
    private void OnChoiceSelected(int newIndex)
    {
        currentLineIndex = newIndex;
    }



    /// <summary>
    /// Call this function from a button press to skip the current line's animation and audio.
    /// </summary>
    //public void SkipCurrentLine()
    //{
    //    if (isLinePlaying)
    //    {
    //        uiView.SkipTypewriter();
    //        narrationPlayer.StopAudio();
    //    }
    //}
    public void SkipCurrentLine()
    {
        // Check the main story flag instead of the old variable
        if (isStoryActive)
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

    public void SkipSegment()
    {
        if (!isStoryActive) return;

        // Stop all current playback
        StopAllCoroutines();
        narrationPlayer.StopAudio();

        // Clear any partially displayed text
        uiView.ClearAllText();

        // Instantly display all remaining lines
        for (int i = currentLineIndex; i < storyToPlay.lines.Count; i++)
        {
            uiView.DisplayLineInstantly(storyToPlay.lines[i]);
        }

        // End the sequence
        isStoryActive = false;
        Debug.Log("Story sequence skipped.");
        OnSequenceFinished?.Invoke();
    }
}