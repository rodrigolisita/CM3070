using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This script instantiates a new object from a prefab and places it
// in the socket whenever the GameObject is enabled.
public class InstantiateInSocketOnEnable : MonoBehaviour
{
    // Assign your socket interactor in the Inspector
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    // Assign the PREFAB of the object you want to create
    [SerializeField]
    private GameObject interactablePrefab;
    
    private XRInteractionManager interactionManager;

    void Awake()
    {
        // Find the scene's Interaction Manager
        interactionManager = FindObjectOfType<XRInteractionManager>();

        if (socketInteractor == null)
        {
            socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        }
    }

    void OnEnable()
    {
        // Check if all references are valid and if the socket is empty
        if (socketInteractor != null && interactablePrefab != null && interactionManager != null && !socketInteractor.hasSelection)
        {
            // Create a new instance of the prefab at the socket's position and rotation
            GameObject newInstance = Instantiate(interactablePrefab, socketInteractor.transform.position, socketInteractor.transform.rotation);
            
            // Get the interactable component from the new instance
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable newInteractable = newInstance.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            if(newInteractable != null)
            {
                // THE FIX: Cast both the interactor and interactable to their interface types.
                interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)socketInteractor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)newInteractable);
            }
            else
            {
                Debug.LogError("The instantiated prefab does not have an XRBaseInteractable component!");
            }
        }
    }
    
}