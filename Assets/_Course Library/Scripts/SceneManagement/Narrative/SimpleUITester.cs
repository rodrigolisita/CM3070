using UnityEngine;

public class SimpleUITester : MonoBehaviour
{
    // Drag the UIStoryView component here in the Inspector
    public UIStoryView storyView;

    // This function will be called by a test button
    public void RunTest()
    {
        if (storyView == null)
        {
            Debug.LogError("StoryView is not assigned!");
            return;
        }

        // We create a temporary StoryLine object directly in code
        StoryLine testLine = new StoryLine();
        
        // We will NOT use localization for this test.
        // We need to temporarily modify UIStoryView to handle this.

        // Let's tell the UIStoryView to display it
        Debug.Log("Tester is calling DisplayTextLine...");
        StartCoroutine(storyView.DisplayTextLine(testLine));
    }
}