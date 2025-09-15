using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;
// Note: Assumes you have the TypewriterTextAnim script from your previous setup.

/// <summary>
/// Manages all UI interactions for the story feed. Instantiates text prefabs,
/// handles typewriter animation, and controls scrolling. Receives commands from StoryPlayer.
/// Automatically finds required UI components in children.
/// </summary>
public class UIStoryView : MonoBehaviour
{
    //[Header("UI Element Prefab")]
    //[Tooltip("The prefab for a single text entry. Must have TypewriterTextAnim and LocalizeStringEvent components.")]
    //[SerializeField] private GameObject textPrefab;

    //[Header("UI References (Optional - Auto-finds if empty)")]
    //[Tooltip("The parent RectTransform where new text objects will be instantiated (the 'Content' object of a Scroll View).")]
    //[SerializeField] private RectTransform contentParent;
    //[Tooltip("The main Scroll Rect component to control scrolling.")]
    //[SerializeField] private ScrollRect scrollRect;

    //private TypewriterTextAnim currentTypewriter;
    
    // These will now be set by the GameManager via InjectReferences
//    private GameObject textPrefab;
//    private RectTransform contentParent;
//    private ScrollRect scrollRect;

//    private TypewriterTextAnim currentTypewriter;

//    private GameObject choiceButtonPrefab;
//    private Transform choiceContainer;

[Header("Internal UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private Transform choiceContainer;

    [Header("External Prefabs")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject choiceButtonPrefab;

    private TypewriterTextAnim currentTypewriter;

    void Awake()
    {
        // Find the components within the same prefab
        scrollRect = GetComponentInChildren<ScrollRect>(true);
        if (scrollRect != null)
        {
            contentParent = scrollRect.content;
        }

        // Find the choice container by its name
        Transform choiceContainerTransform = transform.parent.Find("ChoiceContainer");
        if (choiceContainerTransform != null)
        {
            choiceContainer = choiceContainerTransform;
        }
    }

    /// <summary>
    /// Injects the necessary UI and prefab references from the GameManager.
    /// </summary>
    public void InjectReferences(GameObject scrollViewInstance, GameObject textEntryPrefab, GameObject choiceBtnPrefab)
    {
        // Store prefab references
        this.textPrefab = textEntryPrefab;
        this.choiceButtonPrefab = choiceBtnPrefab;

        // Get components from the instantiated ScrollView
        this.scrollRect = scrollViewInstance.GetComponent<ScrollRect>();
        this.contentParent = this.scrollRect.content;

        // Find or create a container for choice buttons
        // For simplicity, we can create it as a child of the scroll view
        //GameObject choiceContainerObj = new GameObject("ChoiceContainer");
        //choiceContainerObj.transform.SetParent(scrollViewInstance.transform, false);
        //this.choiceContainer = choiceContainerObj.transform;
        // Add a layout group to the container to organize buttons
        //choiceContainerObj.AddComponent<VerticalLayoutGroup>();

        this.choiceContainer = scrollViewInstance.transform.Find("ChoiceContainer");
    }

    
    //void Awake()
    //{
        // Auto-find ScrollRect in children
    //    if (scrollRect == null)
    //    {
    //        scrollRect = GetComponentInChildren<ScrollRect>();
    //        if (scrollRect == null)
    //        {
    //            Debug.LogError("UIStoryView could not find a ScrollRect component in its children.", this);
    //        }
    //    }

        // Auto-find Content parent from the ScrollRect reference
    //    if (contentParent == null && scrollRect != null)
    //    {
    //        contentParent = scrollRect.content;
    //        if (contentParent == null)
    //        {
    //            Debug.LogError("The ScrollRect's 'Content' property is not assigned.", this);
    //        }
    //    }
    //}

    /// <summary>
    /// Displays a single line of text with a typewriter effect.
    /// </summary>
    public IEnumerator DisplayTextLine(StoryLine line)
    {
        // 1. Instantiate prefab and get components
        GameObject newTextInstance = Instantiate(textPrefab, contentParent);
        LocalizeStringEvent localizeEvent = newTextInstance.GetComponent<LocalizeStringEvent>();
        currentTypewriter = newTextInstance.GetComponent<TypewriterTextAnim>();

        // Find the Image component on the prefab instance (assumes there's only one)
        PortraitReference portraitRef = newTextInstance.GetComponentInChildren<PortraitReference>();

        if (portraitRef != null)
        {
            Image portraitImageComponent = portraitRef.GetComponent<Image>(); // Get Image component from the tagged object

            if (line.characterPortrait != null)
            {
                portraitImageComponent.sprite = line.characterPortrait;
                portraitImageComponent.enabled = true;
            }
            else
            {
                portraitImageComponent.enabled = false;
            }
        }
        

        // 2. Load localized text asynchronously
        bool hasTextLoaded = false;
        if (localizeEvent != null && !line.localizedText.IsEmpty)
        {
            Debug.Log("2. Found LocalizeStringEvent. Waiting for text to load..."); // LOG 2
            
            // Subscribe to the event that fires when localization successfully updates the text component.
            localizeEvent.OnUpdateString.AddListener((s) => 
            { 
                Debug.Log("3. OnUpdateString event fired! Text has loaded: " + s); // LOG 3
                hasTextLoaded = true; 
            });

            // Assign the string reference to trigger the asynchronous load operation.
            localizeEvent.StringReference = line.localizedText;
        }
        else
        {
            Debug.LogWarning("LocalizeStringEvent component or LocalizedString reference is missing. Skipping wait.", this);
            hasTextLoaded = true; // Skip wait to avoid infinite loop.
        }

        // Wait here until the UpdatedEvent sets hasTextLoaded to true.
        yield return new WaitUntil(() => hasTextLoaded);

        Debug.Log("4. Wait is over. Starting typewriter animation."); // LOG 4


        // 3. Start typewriter animation
        if (currentTypewriter != null)
        {
            currentTypewriter.SetSpeed(line.charsPerSecond);
            currentTypewriter.StartAnimation();
            //StartCoroutine(ForceScrollDown());
            //StartCoroutine(ForceScrollToTop());
            // Call the scrolling function and pass it a reference to the new text object
            StartCoroutine(ForceScrollToCenter(newTextInstance.GetComponent<RectTransform>()));


            // Wait until the typewriter animation reports completion.
            yield return new WaitUntil(() => currentTypewriter.IsAnimationFinished);
        }
    }

    /// <summary>
    /// Skips the currently playing typewriter animation.
    /// </summary>
    public void SkipTypewriter()
    {
        if (currentTypewriter != null && !currentTypewriter.IsAnimationFinished)
        {
            currentTypewriter.Skip();
        }
    }

    private IEnumerator ForceScrollDown()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// Waits for the UI to update for one frame, then scrolls to the top.
    /// </summary>
    private IEnumerator ForceScrollToTop()
    {
        // Wait for the end of frame so the UI layout system can calculate the new content size.
        yield return new WaitForEndOfFrame();

        // Now that the layout is updated, set the scroll position to the top.
        scrollRect.verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// Calculates the correct scroll position to center the newly added item in the viewport.
    /// </summary>
    private IEnumerator ForceScrollToCenter(RectTransform newItem)
    {
        // Wait for the end of frame so the UI layout system can calculate the new content size and item position.
        yield return new WaitForEndOfFrame();

        // Get the total height of the scrollable content area
        float contentHeight = contentParent.rect.height;
        // Get the height of the visible area
        float viewportHeight = scrollRect.viewport.rect.height;

        // If content is smaller than the viewport, no need to scroll. Go to the top.
        if (contentHeight <= viewportHeight)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            yield break;
        }

        // Calculate the position of the new item's center, relative to the bottom of the content area
        float itemCenterPos = -newItem.anchoredPosition.y;

        // Calculate the target position for the content's anchor to center the item
        float targetContentPos = itemCenterPos - (viewportHeight / 2f);
        
        // Calculate the total scrollable distance
        float scrollableRange = contentHeight - viewportHeight;

        // Normalize the target position to a 0-1 value for the scrollbar
        float normalizedPos = Mathf.Clamp01(targetContentPos / scrollableRange);

        // In Unity's ScrollRect, 1 is the top and 0 is the bottom, so we invert the value
        scrollRect.verticalNormalizedPosition = 1f - normalizedPos;
    }

    public void ClearAllText()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayLineInstantly(StoryLine line)
    {
        // This is a simplified version of DisplayTextLine that works instantly
        GameObject newTextInstance = Instantiate(textPrefab, contentParent);
        var textComponent = newTextInstance.GetComponentInChildren<TextMeshProUGUI>();
        
        // This assumes your TextEntryTemplate has the necessary components
        // like LocalizeStringEvent to handle the text assignment.
        LocalizeStringEvent localizeEvent = newTextInstance.GetComponentInChildren<LocalizeStringEvent>();
        if (localizeEvent != null)
        {
            localizeEvent.StringReference = line.localizedText;
        }

        // We can also add portrait logic here if needed
        Image portraitImage = newTextInstance.GetComponentInChildren<Image>();
        if (portraitImage != null)
        {
            if (line.characterPortrait != null)
            {
                portraitImage.sprite = line.characterPortrait;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }
    }

    /// <summary>
    /// Creates buttons for each choice and waits for a selection.
    /// </summary>
    public IEnumerator DisplayChoices(List<ChoiceData> choices, Action<int> onChoiceSelectedCallback)
    {
        // Clear old choices from container
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        bool choiceMade = false;
        choiceContainer.gameObject.SetActive(true); // Make sure the container is visible

        // Create a button for each choice
        foreach (ChoiceData choice in choices)
        {
            GameObject buttonInstance = Instantiate(choiceButtonPrefab, choiceContainer);

            // Set button text
            TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choice.choiceText;
            }

            // Add listener to button click event
            Button button = buttonInstance.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                choiceMade = true;
                onChoiceSelectedCallback(choice.nextLineIndex);
            });
        }

        // Wait here until a button click sets choiceMade to true
        yield return new WaitUntil(() => choiceMade);
        
        // Clean up the buttons after a choice has been made
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        choiceContainer.gameObject.SetActive(false);
    }
}