using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DraggableSlicerPlane : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private DataSlicer dataSlicer;

    private float minZ, maxZ;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        dataSlicer = GetComponentInParent<DataSlicer>();

        if (dataSlicer == null)
        {
            Debug.LogError("DraggableSlicerPlane requires a DataSlicer component on its parent or the same GameObject.");
            enabled = false;
        }
    }

    public void InitializeBounds(float newMinZ, float newMaxZ)
    {
        minZ = newMinZ;
        maxZ = newMaxZ;
    }

    void Update()
    {
        // Only do work if the plane is being actively held
        if (grabInteractable.isSelected)
        {
            // Read the current Z position, which is being set by the XRGrabInteractable itself
            float currentZ = transform.position.z;

            // Clamp the value just in case the hand goes out of the data bounds
            float clampedZ = Mathf.Clamp(currentZ, minZ, maxZ);

            // If the XR system moved the plane out of bounds, force it back.
            // This ensures the plane doesn't get stuck at the edges.
            if (Mathf.Approximately(clampedZ, currentZ) == false)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, clampedZ);
            }

            // Calculate the normalized value (0 to 1) from the clamped position
            float normalizedValue = Mathf.InverseLerp(minZ, maxZ, clampedZ);
            
            // Notify the DataSlicer to update the UI and the visualization
            dataSlicer.SetSliceFromNormalizedValue(normalizedValue, true);
        }
    }
}

