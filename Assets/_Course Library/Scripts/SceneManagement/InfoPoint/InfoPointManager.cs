using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Required for lists

[System.Serializable]
public class InfoPointLink
{
    //[Tooltip("The button in the scene that will trigger the story.")]
    //private Button triggerButton;
    [Tooltip("The EXACT name of the trigger Button GameObject in the scene.")]
    public string triggerButtonName = "InfoButton"; // Changed from Button to string

    [Tooltip("The InfoPointScreen GameObject that this button will activate.")]
    public GameObject infoPointScreen;

    [Tooltip("The list of Narrative Segments that will play in order.")]
    public List<NarrativeSegment> segmentsToPlay;

    public bool startAutomatically = false;

    [Tooltip("If checked, the 'Close' button will be visible for this story.")]
    public bool allowToClose = true;
}

public class InfoPointManager : MonoBehaviour
{
    [Header("Connections")]
    [Tooltip("Create a list linking each button to a screen and a narrative segment.")]
    public List<InfoPointLink> infoPoints;

    private GameObject currentScreenInstance;
    private List<NarrativeSegment> activeSegmentList;
    private int currentSegmentIndex;

    void Start()
    {
        // First, deactivate all linked screens to ensure a clean start
        foreach (var link in infoPoints)
        {
            if (link.infoPointScreen == null) continue;

            // Find the child object named "-- INTERFACE --"
            Transform interfaceTransform = link.infoPointScreen.transform.Find("-- INTERFACE --");

            if (interfaceTransform != null)
            {
                GameObject interfaceObject = interfaceTransform.gameObject;
                interfaceObject.SetActive(false); // Deactivate it at the start
            }
        }

        // Loop through each link you created in the Inspector
        foreach (var link in infoPoints)
        {
            GameObject screenToActivate = link.infoPointScreen;
            List<NarrativeSegment> segmentsForThisButton = link.segmentsToPlay;
            bool canBeClosedForThisButton = link.allowToClose;

            //GameObject triggerButtonGO = GameObject.Find(link.triggerButtonName);
            //Button triggerButton = triggerButtonGO.GetComponent<Button>();
            // Find the button component within the specified infoPointScreen's children
            Button triggerButton = null;
            if (screenToActivate != null)
            {
                // We use GetComponentInChildren<Button>(true) which can find components on inactive objects
                Button[] allButtons = screenToActivate.GetComponentsInChildren<Button>(true);
                foreach (Button btn in allButtons)
                {
                    if (btn.name == link.triggerButtonName)
                    {
                        triggerButton = btn;
                        break;
                    }
                }
            }

            // Add a listener to the button's click event
            if (triggerButton != null)
            {
 //               triggerButton.onClick.AddListener(() => { ActivateAndPlayStory(screenToActivate, segmentsForThisButton); });
                triggerButton.onClick.AddListener(() => { ActivateAndPlayStory(screenToActivate, segmentsForThisButton, canBeClosedForThisButton); });

            }
            // If this link is set to start automatically, trigger it now.
            if (link.startAutomatically)
            {
                triggerButton.onClick.Invoke();
            }
            
        }
    }

//    public void ActivateAndPlayStory(GameObject screenInstance, List<NarrativeSegment> segments)
    public void ActivateAndPlayStory(GameObject screenInstance, List<NarrativeSegment> segments, bool allowClose)
    {
        // Store the active list and reset the index
        activeSegmentList = segments;
        currentSegmentIndex = 0;

        // If a different story screen is already open, close it first
        foreach (var link in infoPoints)
        {
            // If this is a different InfoPoint from the one we want to activate...
            if (link.infoPointScreen != screenInstance)
            {
                // ...find its interface child and deactivate it.
                Transform interfaceTransform = link.infoPointScreen.transform.Find("-- INTERFACE --");
                if (interfaceTransform != null)
                {
                    interfaceTransform.gameObject.SetActive(false);
                }
            }
        }

        // Activate the new screen
        currentScreenInstance = screenInstance;
        currentScreenInstance.SetActive(true);

         ConfigureNavigationButtons();
         SceneStoryController controller = currentScreenInstance.GetComponentInChildren<SceneStoryController>();
        if (controller != null)
        {
            // Pass the allowClose flag to the controller
            controller.SetAllowClose(allowClose);
        }
        
        PlayCurrentSegment();
    }

    /// <summary>
    /// Called by the 'Continue' button.
    /// </summary>
    public void PlayNext()
    {
        if (activeSegmentList != null && currentSegmentIndex < activeSegmentList.Count - 1)
        {
            currentSegmentIndex++;
            PlayCurrentSegment();
        }
    }

    /// <summary>
    /// Called by the 'Back' button.
    /// </summary>
    public void PlayPrevious()
    {
        if (activeSegmentList != null && currentSegmentIndex > 0)
        {
            currentSegmentIndex--;
            PlayCurrentSegment();
        }
    }

    private void PlayCurrentSegment()
    {
        // Find the controller and play the segment at the current index
        SceneStoryController controller = currentScreenInstance.GetComponentInChildren<SceneStoryController>();
        if (controller != null)
        {
            bool hasNext = currentSegmentIndex < activeSegmentList.Count - 1;
            bool hasPrevious = currentSegmentIndex > 0;
            controller.StartNarrativeSegment(activeSegmentList[currentSegmentIndex], hasNext, hasPrevious);

        }
    }

    private void ConfigureNavigationButtons()
    {
        if (currentScreenInstance == null) return;

        Button continueButton = null;
        Button backButton = null;

        // Find the buttons by name
        Button[] allButtons = currentScreenInstance.GetComponentsInChildren<Button>();
        foreach (Button btn in allButtons)
        {
            if (btn.name == "ContinueButton")
            {
                continueButton = btn;
            }
            else if (btn.name == "BackButton")
            {
                backButton = btn;
            }
        }

        // Connect the Continue button's OnClick event
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners(); // Clear old listeners
            continueButton.onClick.AddListener(PlayNext);
        }

        // Connect the Back button's OnClick event
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Clear old listeners
            backButton.onClick.AddListener(PlayPrevious);
        }
    }
   
    
}