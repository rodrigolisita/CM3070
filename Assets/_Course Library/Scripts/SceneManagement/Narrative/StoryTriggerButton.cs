using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StoryTriggerButton : MonoBehaviour
{
    [Tooltip("The Narrative Segment that this button should trigger.")]
    public NarrativeSegment segmentToTrigger;

    private Button triggerButton;
    private GameManager gameManager;

    void Start()
    {
        // Find the single instance of the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        
        // Get the Button component on this GameObject
        triggerButton = GetComponent<Button>();
        
        // Add a listener to the button's click event
        triggerButton.onClick.AddListener(TriggerStorySegment);
    }

    void TriggerStorySegment()
    {
        if (segmentToTrigger != null && gameManager != null)
        {
            // Tell the GameManager to start the specific segment
            gameManager.StartNarrativeSegment(segmentToTrigger);
        }
        else
        {
            Debug.LogWarning("StoryTriggerButton is missing a Segment to trigger or cannot find the GameManager.", this);
        }
    }
}