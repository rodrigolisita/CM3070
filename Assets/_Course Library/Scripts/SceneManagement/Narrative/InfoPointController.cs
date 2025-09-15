using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class InfoPointController : MonoBehaviour
{
    [Header("Content")]
    [Tooltip("The Narrative Segment this info point will display.")]
    public NarrativeSegment narrativeSegment;

    [Header("Settings")]
    [Tooltip("If checked, this info point will start its narrative as soon as the scene loads.")]
    public bool startAutomatically = false;

    [Header("UI References")]
    [Tooltip("The Canvas that contains the story UI for this info point.")]
    public Canvas storyCanvas;
    [Tooltip("The button that allows the user to skip the narrative.")]
    public GameObject skipButton;
    [Tooltip("The button that appears after the narrative to let the user replay it.")]
    public GameObject playAgainButton;
    
    // Internal reference to the StoryPlayer inside the canvas
    private StoryPlayer storyPlayer;

    void Awake()
    {
        // Find the StoryPlayer within this prefab's children. 
        // The 'true' allows it to find it even if the canvas is inactive.
        storyPlayer = GetComponentInChildren<StoryPlayer>(true);
        if (storyPlayer != null)
        {
            // Subscribe to the event that fires when the story is over
            storyPlayer.OnSequenceFinished += HandleSequenceFinished;
        }

        // Ensure the canvas and all buttons are hidden at the start
        if (storyCanvas != null) storyCanvas.gameObject.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false);
        if (playAgainButton != null) playAgainButton.SetActive(false);
    }

    void Start()
    {
        // If the checkbox is ticked, start the story immediately.
        if (startAutomatically)
        {
            StartStory();
        }
    }

    // A simple way to trigger the story. Works with a mouse click or a VR raycast click
    private void OnMouseDown()
    {
        // Don't do anything if it's already playing automatically or the UI is already visible.
        if (startAutomatically || (storyCanvas != null && storyCanvas.gameObject.activeInHierarchy)) return;

        StartStory();
    }

    public void StartStory()
    {
        // Clean up any text from the previous run
        CleanupPreviousStory();
    if (narrativeSegment != null && storyPlayer != null && storyCanvas != null)
    {
        // Show the UI and the correct buttons
        storyCanvas.gameObject.SetActive(true);
        if (skipButton != null) skipButton.SetActive(true);
        if (playAgainButton != null) playAgainButton.SetActive(false);

        storyPlayer.PlayStory(narrativeSegment.dialogue);
    }
    else
    {
        Debug.LogError("InfoPoint is not configured correctly!", this);
    }
}

    public void CloseStory()
    {
        if (storyCanvas != null)
        {
            if(storyPlayer != null)
            {
                storyPlayer.StopStorySequence();
            }
            storyCanvas.gameObject.SetActive(false);
        }
    }

    public void ReplayStory()
    {
        StartStory();
    }

    // It's good practice to unsubscribe from events when the object is destroyed
    private void OnDestroy()
    {
        if (storyPlayer != null)
        {
            storyPlayer.OnSequenceFinished -= HandleSequenceFinished;
        }
    }

    public void SkipCurrentStory()
    {
        // This function simply finds the StoryPlayer and tells it to skip.
        if (storyPlayer != null)
        {
            storyPlayer.SkipSegment();
        }
    }

     private void CleanupPreviousStory()
    {
        // Find the UI view and tell it to clear its content
        UIStoryView uiView = GetComponentInChildren<UIStoryView>(true);
        if (uiView != null)
        {
            uiView.ClearAllText();
        }
    }

    /// <summary>
    /// This function is called by the StoryPlayer's OnSequenceFinished event.
    /// </summary>
    private void HandleSequenceFinished()
    {
        // Hide the skip button and show the play again button
        if (skipButton != null) skipButton.SetActive(false);
        if (playAgainButton != null) playAgainButton.SetActive(true);

        // Start the coroutine to handle the post-segment action
        StartCoroutine(ExecutePostSegmentAction());
    }

    /// <summary>
    /// Waits for a delay, then executes the final action from the NarrativeSegment.
    /// </summary>
    private IEnumerator ExecutePostSegmentAction()
    {
        // Wait for the delay specified in the asset
        yield return new WaitForSeconds(narrativeSegment.delayBeforeAction);

        // Perform the action defined in the NarrativeSegment
        switch (narrativeSegment.actionOnFinish)
        {
            case PostSegmentAction.LoadScene:
                if (!string.IsNullOrEmpty(narrativeSegment.sceneToLoad))
                {
                    // Before loading a new scene, you might want to fade out
                    // For now, we'll just load it directly.
                    SceneManager.LoadScene(narrativeSegment.sceneToLoad);
                }
                break;
            
            case PostSegmentAction.DoNothing:
                // Do nothing further. The user can click replay or close.
                Debug.Log("Segment finished. Action: DoNothing.");
                break;
        }
    }
}