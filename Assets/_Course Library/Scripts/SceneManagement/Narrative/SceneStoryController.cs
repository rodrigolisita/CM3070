using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// The main controller in the scene. It loads a NarrativeSegment,
/// builds the required UI from a layout blueprint, and starts the story.
/// </summary>
public class SceneStoryController : MonoBehaviour
{
    [Header("Content")]
    [Tooltip("The Narrative Segment to load and play when the scene starts.")]
    public NarrativeSegment segmentToStart;

    //[Header("Settings")]
    //[Tooltip("If checked, this info point will start its narrative as soon as the scene loads.")]
    //public bool startAutomatically = false;

      [Header("UI References")]
    //[Tooltip("The main world-space canvas to instantiate the story UI onto.")]
    //public Canvas mainCanvas;
    [Tooltip("The parent object to instantiate the story UI into.")]
    public Transform storyUiParent;

    //[Header("Transitions")]
    //[Tooltip("The Renderer for the VR fade screen quad/sphere.")]
    //public Renderer fadeScreenRenderer;

    [Header("UI Elements")]
    public GameObject playAgainButton;    
    [Tooltip("The 'Continue' button to go to the next segment.")]
    public GameObject continueButton;
    [Tooltip("The 'Back' button to go to the previous segment.")]
    public GameObject backButton;    
    [Tooltip("The 'Close' button to close the canvas.")]
    public GameObject closeButton;    

    // Internal reference to the currently active story system
    private NarrativeSegment currentSegment;
    private StoryPlayer currentStoryPlayerInstance;
    private GameObject currentScrollViewInstance;
    private int currentSegmentIndex;
    private bool hasNextSegment;
    private bool hasPreviousSegment;

//    void Start()
//    {
//        if (segmentToStart != null && startAutomatically)
//        {
//            StartNarrativeSegment(segmentToStart);
//        }
//    }

//    public void StartNarrativeSegment(NarrativeSegment segment, int segmentIndex)
    public void StartNarrativeSegment(NarrativeSegment segment, bool hasNext, bool hasPrevious)
    {
        // Hide the button at the start of a new segment
        if (playAgainButton != null)
        {
            playAgainButton.SetActive(false);
        }
        if (continueButton != null)
        {
            continueButton.SetActive(hasNext);
        }
        if (backButton != null)
        {
            backButton.SetActive(hasPrevious);
        }

        // 1. Clean up any story that is currently running.
        CleanupCurrentStory();

        if (segment == null || segment.uiLayout == null || storyUiParent == null)
        {
            Debug.LogError("Cannot start narrative: Segment, UI Layout, or Story UI Parent is not assigned.");
            return;
        }

        currentSegment = segment; // Store the current segment
        hasNextSegment = hasNext;
        hasPreviousSegment = hasPrevious;

        // 2. Get the UI blueprint from the segment
        StoryUILayout layout = segment.uiLayout;

        // 3. Build the UI: Instantiate the ScrollView from the blueprint
        currentScrollViewInstance = Instantiate(layout.scrollViewPrefab, storyUiParent);

        // 3.5. Ensure the newly created UI is active before we use it.
        currentScrollViewInstance.SetActive(true);

        // 4. Reset the ScrollView's transform to fill the canvas
        RectTransform scrollRectTransform = currentScrollViewInstance.GetComponent<RectTransform>();
        if (scrollRectTransform != null)
        {
            scrollRectTransform.anchorMin = new Vector2(0, 0); // Bottom-left corner
            scrollRectTransform.anchorMax = new Vector2(1, 1); // Top-right corner
            scrollRectTransform.offsetMin = Vector2.zero; // Distance from anchors
            scrollRectTransform.offsetMax = Vector2.zero;
            scrollRectTransform.localScale = Vector3.one;
            scrollRectTransform.localRotation = Quaternion.identity;
        }

        // 5. Build the Logic System
        GameObject storySystemObject = new GameObject("StorySystem (Runtime)");
        storySystemObject.transform.SetParent(currentScrollViewInstance.transform, false);

        // Add components
        UIStoryView uiView = storySystemObject.AddComponent<UIStoryView>();
        NarrationPlayer narrationPlayer = storySystemObject.AddComponent<NarrationPlayer>();
        storySystemObject.AddComponent<AudioSource>();
        currentStoryPlayerInstance = storySystemObject.AddComponent<StoryPlayer>();

        // Subscribe to the "finished" event before starting
        currentStoryPlayerInstance.OnSequenceFinished += HandleStoryFinished;

        // 6. Connect the systems
        uiView.InjectReferences(currentScrollViewInstance, layout.textEntryPrefab, layout.choiceButtonPrefab);
        currentStoryPlayerInstance.InjectReferences(uiView, narrationPlayer);

        // 7. Start the story
        currentStoryPlayerInstance.PlayStory(segment.dialogue);
    }

    /// <summary>
    /// This function is called when the StoryPlayer's OnSequenceFinished event fires.
    /// </summary>
    private void HandleStoryFinished()
    {
        Debug.Log("GameManager received OnSequenceFinished event.");

        // Unsubscribe from the event to prevent issues
        if (currentStoryPlayerInstance != null)
        {
            currentStoryPlayerInstance.OnSequenceFinished -= HandleStoryFinished;
        }

        // Show the button now that the story is over
        if (playAgainButton != null)
        {
            playAgainButton.SetActive(true);
        }
        //if (continueButton != null)
        //{
        //    continueButton.SetActive(true);
        //}

        // Start a new coroutine to handle the post-segment action with a delay
        StartCoroutine(ExecutePostSegmentAction());

    }

    private IEnumerator ExecutePostSegmentAction()
    {
        float delay = currentSegment.delayBeforeAction;
        float fadeDuration = 1.0f; // You can make this a public variable

        // Wait for any initial delay before the fade starts
        if (delay > fadeDuration)
        {
            yield return new WaitForSeconds(delay - fadeDuration);
        }

        // Perform the action defined in the NarrativeSegment
        switch (currentSegment.actionOnFinish)
        {
            case PostSegmentAction.LoadScene:
                if (!string.IsNullOrEmpty(currentSegment.sceneToLoad))
                {
                    // Start the fade out
    //                yield return StartCoroutine(FadeOut(fadeDuration));
                    SceneManager.LoadScene(currentSegment.sceneToLoad);
                }
                break;

            case PostSegmentAction.DoNothing:
                // The screen is now black and the UI is gone.
                // You might want to fade back in here if the game continues in the same scene.
                Debug.Log("Segment finished. Action: DoNothing.");
                break;
        }
    }
    
  
//    private IEnumerator FadeOut(float duration)
//    {
        // Get the material instance
//        Material material = fadeScreenRenderer.material;
        
        // Make sure it's visible before we start
//        fadeScreenRenderer.enabled = true;
//        SetAlpha(material, 0f);

//        float elapsedTime = 0f;
//        while (elapsedTime < duration)
//        {
//            elapsedTime += Time.deltaTime;
//            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
//            SetAlpha(material, newAlpha);
//            yield return null;
//        }

        // Ensure it's fully black
//        SetAlpha(material, 1f);
//    }

    private void SetAlpha(Material mat, float alpha)
    {
        Color currentColor = mat.color;
        currentColor.a = alpha;
        mat.color = currentColor;
    }
    
    /// <summary>
    /// Finds and destroys the currently active story UI and logic.
    /// </summary>
    public void CleanupCurrentStory()
    {
        // If a story is currently running, stop its logic.
        if (currentStoryPlayerInstance != null)
        {
            currentStoryPlayerInstance.StopStorySequence();
        }

        // If a ScrollView was created, destroy it.
        if (currentScrollViewInstance != null)
        {
            Destroy(currentScrollViewInstance);
        }
    }

    /// <summary>
    /// Replays the last narrative segment that was played.
    /// This function should be called by the 'Play Again' button.
    /// </summary>
    public void ReplayCurrentSegment()
    {
        // Check if there is a segment to replay
        if (currentSegment != null)
        {
            // Simply call the main function again with the same segment data
            StartNarrativeSegment(currentSegment,hasNextSegment,hasPreviousSegment);
        }
    }

    public void SkipCurrentStory()
    {
        if (currentStoryPlayerInstance != null)
        {
            currentStoryPlayerInstance.SkipSegment();
        }
    }
    
    /// <summary>
    /// Sets the visibility of the Close button. Called by the InfoPointManager.
    /// </summary>
    public void SetAllowClose(bool isAllowed)
    {
        if (closeButton != null)
        {
            closeButton.SetActive(isAllowed);
        }
    }
    
}