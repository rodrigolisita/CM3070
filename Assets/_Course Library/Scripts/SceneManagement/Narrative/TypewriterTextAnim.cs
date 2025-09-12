using UnityEngine;
using TMPro;

/// <summary>
/// Manages the typewriter effect for a TextMeshProUGUI component.
/// Animation must be initiated by calling StartAnimation().
/// </summary>
public class TypewriterTextAnim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private float charsPerSecond = 20f;
    private float startTime;

    // Default IsAnimationFinished to true so TextFeedManager doesn't skip past it
    // if StartAnimation() hasn't been called yet.
    public bool IsAnimationFinished { get; private set; } = true;

    private bool animationHasStarted = false;

    void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Initializes and starts the typewriter animation.
    /// Call this after ensuring the text content has been loaded.
    /// </summary>
    public void StartAnimation()
    {
        // Reset all state variables for a new animation run
        startTime = Time.time;
        IsAnimationFinished = false;
        animationHasStarted = true;
        textMesh.maxVisibleCharacters = 0;
    }

    void Update()
    {
        // Do nothing if the animation hasn't been started or is already finished.
        if (!animationHasStarted || IsAnimationFinished)
        {
            return;
        }

        // Calculate visible characters based on time elapsed since StartAnimation() was called.
        int totalChars = textMesh.textInfo.characterCount;
        int visibleChars = Mathf.Clamp(Mathf.FloorToInt(charsPerSecond * (Time.time - startTime)), 0, totalChars);
        textMesh.maxVisibleCharacters = visibleChars;

        // Mark as finished when all characters are visible.
        if (visibleChars >= totalChars)
        {
            IsAnimationFinished = true;
            animationHasStarted = false; // Stop processing updates
        }
    }

    public void Skip()
    {
        // Only skip if the component exists and animation is running.
        if (textMesh != null)
        {
            textMesh.maxVisibleCharacters = textMesh.textInfo.characterCount;
        }
        IsAnimationFinished = true;
        animationHasStarted = false;
    }

    public void SetSpeed(float speed)
    {
        this.charsPerSecond = speed;
    }
}