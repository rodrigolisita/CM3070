using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
// Note: Assumes you have the TypewriterTextAnim script from your previous setup.

/// <summary>
/// Manages all UI interactions for the story feed. Instantiates text prefabs,
/// handles typewriter animation, and controls scrolling. Receives commands from StoryPlayer.
/// Automatically finds required UI components in children.
/// </summary>
public class UIStoryView : MonoBehaviour
{
    [Header("UI Element Prefab")]
    [Tooltip("The prefab for a single text entry. Must have TypewriterTextAnim and LocalizeStringEvent components.")]
    [SerializeField] private GameObject textPrefab;

    [Header("UI References (Optional - Auto-finds if empty)")]
    [Tooltip("The parent RectTransform where new text objects will be instantiated (the 'Content' object of a Scroll View).")]
    [SerializeField] private RectTransform contentParent;
    [Tooltip("The main Scroll Rect component to control scrolling.")]
    [SerializeField] private ScrollRect scrollRect;

    private TypewriterTextAnim currentTypewriter;

    // --- NEW CODE START ---
    void Awake()
    {
        // Auto-find ScrollRect in children
        if (scrollRect == null)
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogError("UIStoryView could not find a ScrollRect component in its children.", this);
            }
        }

        // Auto-find Content parent from the ScrollRect reference
        if (contentParent == null && scrollRect != null)
        {
            contentParent = scrollRect.content;
            if (contentParent == null)
            {
                Debug.LogError("The ScrollRect's 'Content' property is not assigned.", this);
            }
        }
    }
    // --- NEW CODE END ---

    /// <summary>
    /// Displays a single line of text with a typewriter effect.
    /// </summary>
    public IEnumerator DisplayTextLine(StoryLine line)
    {
        // 1. Instantiate prefab and get components
        GameObject newTextInstance = Instantiate(textPrefab, contentParent);
        LocalizeStringEvent localizeEvent = newTextInstance.GetComponent<LocalizeStringEvent>();
        currentTypewriter = newTextInstance.GetComponent<TypewriterTextAnim>();

        // 2. Load localized text asynchronously
        bool hasTextLoaded = false;
        if (localizeEvent != null)
        {
            localizeEvent.OnUpdateString.AddListener((_) => { hasTextLoaded = true; });
            localizeEvent.StringReference = line.localizedText;
        }
        else
        {
            Debug.LogWarning("Text prefab missing LocalizeStringEvent component.", this);
            hasTextLoaded = true; // Skip wait
        }

        yield return new WaitUntil(() => hasTextLoaded);

        // 3. Start typewriter animation
        if (currentTypewriter != null)
        {
            currentTypewriter.SetSpeed(line.charsPerSecond);
            currentTypewriter.StartAnimation();
            StartCoroutine(ForceScrollDown());

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
}