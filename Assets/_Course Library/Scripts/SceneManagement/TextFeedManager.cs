using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

/// <summary>
/// A helper class to group a localized string with its specific animation speed and post-display delay.
/// </summary>
[System.Serializable]
public class StoryParagraph
{
    [Tooltip("The localization key for the text.")]
    public LocalizedString localizedString;

    [Tooltip("How fast the text appears, in characters per second.")]
    public float charsPerSecond = 25f;

    [Tooltip("The pause in seconds after this paragraph has finished typing.")]
    public float delayAfter = 1.5f;
}

/// <summary>
/// Manages the sequential display of localized text paragraphs with a typewriter effect
/// in a scrollable UI view. It handles asynchronous text loading before starting animations.
/// Assumes the textPrefab has TypewriterTextAnim and LocalizeStringEvent components.
/// </summary>
public class TextFeedManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The prefab for a single text entry. Must have TextMeshProUGUI, TypewriterTextAnim, and LocalizeStringEvent components.")]
    [SerializeField] private GameObject textPrefab;
    [Tooltip("The parent RectTransform where new text objects will be instantiated (the 'Content' object of a Scroll View).")]
    [SerializeField] private RectTransform contentParent;
    [Tooltip("The main Scroll Rect component to control scrolling.")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Story Content")]
    [Tooltip("The list of story paragraphs to be displayed in order. Each can have a custom speed and delay.")]
    [SerializeField] private StoryParagraph[] storyParagraphs;

    private TypewriterTextAnim currentTypewriter;

    void Start()
    {
        if (textPrefab == null || contentParent == null || scrollRect == null)
        {
            Debug.LogError("TextFeedManager is missing one or more UI references. Please assign them in the Inspector.");
            return;
        }

        StartCoroutine(DisplayStory());
    }

    /// <summary>
    /// Coroutine to display all story paragraphs sequentially.
    /// It ensures text localization completes before starting the typewriter animation for each entry.
    /// </summary>
    private IEnumerator DisplayStory()
    {
        foreach (StoryParagraph paragraph in storyParagraphs)
        {
            // 1. INSTANTIATE PREFAB
            GameObject newTextInstance = Instantiate(textPrefab, contentParent);

            // 2. LOAD LOCALIZED TEXT ASYNCHRONOUSLY
            bool hasTextLoaded = false;
            LocalizeStringEvent localizeEvent = newTextInstance.GetComponent<LocalizeStringEvent>();

            if (localizeEvent != null)
            {
                // Subscribe to the event that fires when localization successfully updates the text component.
                localizeEvent.OnUpdateString.AddListener((_) => { hasTextLoaded = true; });

                // Assign the string reference to trigger the asynchronous load operation.
                localizeEvent.StringReference = paragraph.localizedString;
            }
            else
            {
                Debug.LogWarning("Text prefab is missing LocalizeStringEvent component. Assuming text is available immediately.", this);
                hasTextLoaded = true; // Skip wait to avoid infinite loop if component is missing.
            }

            // Wait here until the UpdatedEvent sets hasTextLoaded to true.
            yield return new WaitUntil(() => hasTextLoaded);

            // 3. START TYPEWRITER ANIMATION (Now that text content is guaranteed to be loaded)
            currentTypewriter = newTextInstance.GetComponent<TypewriterTextAnim>();
            if (currentTypewriter != null)
            {
                // Configure and start the animation using the methods created in TypewriterTextAnim.
                currentTypewriter.SetSpeed(paragraph.charsPerSecond);
                currentTypewriter.StartAnimation(); // Explicitly start animation.

                // Scroll to bottom as the new text starts appearing.
                StartCoroutine(ForceScrollDown());

                // Wait until the typewriter animation reports completion.
                yield return new WaitUntil(() => currentTypewriter.IsAnimationFinished);
            }
            else
            {
                Debug.LogError("Text prefab is missing TypewriterTextAnim component.", this);
            }

            // 4. POST-PARAGRAPH DELAY
            // Use the specific delay from the current paragraph object.
            yield return new WaitForSeconds(paragraph.delayAfter);
        }
    }

    /// <summary>
    /// Skips the currently playing typewriter animation, causing it to display instantly.
    /// </summary>
    public void SkipCurrentAnimation()
    {
        if (currentTypewriter != null && !currentTypewriter.IsAnimationFinished)
        {
            currentTypewriter.Skip();
        }
    }

    /// <summary>
    /// Forces the scroll view to snap to the bottom. Called after a frame delay
    /// to allow the UI layout to update with the new text object.
    /// </summary>
    private IEnumerator ForceScrollDown()
    {
        // Wait for the end of frame so the UI layout system can calculate the new content size.
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}