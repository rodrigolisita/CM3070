using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required for Button components

public class ScreenTransitionManager : MonoBehaviour
{
    // Assign these in the Inspector:
    [Tooltip("All pages/screens to transition between.")]
    public GameObject[] pages;

    [Tooltip("The speed of the fade transition in seconds.")]
    public float fadeDuration = 0.5f;

    // --- Internal Variables ---
    private int currentPageIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // Initialize all pages. Ensure only the first page is visible initially.
        for (int i = 0; i < pages.Length; i++)
        {
            // Get the CanvasGroup component. Add one if it doesn't exist.
            CanvasGroup cg = pages[i].GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = pages[i].AddComponent<CanvasGroup>();
            }

            if (i == currentPageIndex)
            {
                // First page starts visible and interactive.
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
                pages[i].SetActive(true);
            }
            else
            {
                // Other pages start invisible and non-interactive.
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
                pages[i].SetActive(false);
            }
        }
    }

    // Public method to be called by the "Continue" button.
    public void GoToNextPage()
    {
        if (isTransitioning || currentPageIndex >= pages.Length - 1)
        {
            return; // Exit if already changing screens or on the last page.
        }

        // Start the transition process from current page to next page.
        StartCoroutine(FadeTransition(currentPageIndex, currentPageIndex + 1));
    }

    // Public method to be called by a "Back" button (optional).
    public void GoToPreviousPage()
    {
        if (isTransitioning || currentPageIndex <= 0)
        {
            return; // Exit if already changing screens or on the first page.
        }

        // Start the transition process from current page to previous page.
        StartCoroutine(FadeTransition(currentPageIndex, currentPageIndex - 1));
    }

    // --- Coroutine for Fading Logic ---
    private IEnumerator FadeTransition(int fromIndex, int toIndex)
    {
        isTransitioning = true;

        // Get CanvasGroups for both pages
        CanvasGroup fromPageCG = pages[fromIndex].GetComponent<CanvasGroup>();
        CanvasGroup toPageCG = pages[toIndex].GetComponent<CanvasGroup>();

        // --- Step 1: Fade Out Current Page ---
        // Make sure "from" page is interactive during fade out (or set to false immediately)
        fromPageCG.interactable = false;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fromPageCG.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null; // Wait for the next frame
        }
        fromPageCG.alpha = 0f;
        fromPageCG.blocksRaycasts = false;
        pages[fromIndex].SetActive(false); // Disable old page object completely

        // --- Step 2: Fade In New Page ---
        pages[toIndex].SetActive(true); // Enable new page object
        toPageCG.alpha = 0f; // Ensure new page starts fully transparent

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            toPageCG.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null; // Wait for the next frame
        }
        toPageCG.alpha = 1f;
        toPageCG.interactable = true;
        toPageCG.blocksRaycasts = true;

        // Update current state
        currentPageIndex = toIndex;
        isTransitioning = false;
    }
}