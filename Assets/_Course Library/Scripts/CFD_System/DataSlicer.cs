using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CFD_DataProvider))]
public class DataSlicer : MonoBehaviour
{
    [Header("Slicing Control")]
    public Slider positionSlider;
    public Slider thicknessSlider; // Slider for thickness
    [Tooltip("The thickness of the slice to be displayed.")]
    public float sliceThickness = 0.1f;

    [Header("Direct Interaction")]
    [Tooltip("Drag the physical slicer plane object here.")]
    public DraggableSlicerPlane slicerPlane;

    public List<DataPoint> SlicedData { get; private set; } = new List<DataPoint>();
    public event System.Action OnSliceUpdated;
    
    private CFD_DataProvider dataProvider;
    private float minZ, maxZ;
    private bool isUpdatingFromPlane = false; // Flag to prevent update loops

    void Awake()
    {
        dataProvider = GetComponent<CFD_DataProvider>();
    }

    private void OnEnable()
    {
        dataProvider.OnDataLoaded += OnFullDataLoaded;
        if (positionSlider != null) positionSlider.onValueChanged.AddListener(OnSlicePositionChanged);
        if (thicknessSlider != null) thicknessSlider.onValueChanged.AddListener(OnSliceThicknessChanged);
    }

    private void OnDisable()
    {
        dataProvider.OnDataLoaded -= OnFullDataLoaded;
        if (positionSlider != null) positionSlider.onValueChanged.RemoveListener(OnSlicePositionChanged);
        if (thicknessSlider != null) thicknessSlider.onValueChanged.RemoveListener(OnSliceThicknessChanged);
    }

    private void OnFullDataLoaded()
    {
        if (!dataProvider.IsDataReady) return;
        minZ = dataProvider.DataPoints.Min(p => p.position.z);
        maxZ = dataProvider.DataPoints.Max(p => p.position.z);

        if (slicerPlane != null)
        {
            slicerPlane.InitializeBounds(minZ, maxZ);
        }

        if (positionSlider != null)
        {
            positionSlider.value = 0.0f;
        }
        if (thicknessSlider != null)
        {
            thicknessSlider.value = 0.001f; // Set a small initial thickness
        }

        SetSliceFromNormalizedValue(positionSlider.value, false);

        //OnSlicePositionChanged(positionSlider.value);
    }
    
    // Function called by the thickness slider
    public void OnSliceThicknessChanged(float sliderValue)
    {
        // Map the slider's 0-1 value to a reasonable range, e.g., 0.001 to 0.1
        //sliceThickness = Mathf.Lerp(0.001f, 0.2f, sliderValue);
        sliceThickness = Mathf.Lerp(minZ, maxZ, sliderValue);

        // Redraw the slice with the new thickness
        OnSlicePositionChanged(positionSlider.value);
    }

    // Called when the UI SLIDER is moved by the user
    public void OnSlicePositionChanged(float sliderValue)
    {
        if (isUpdatingFromPlane) return; // Prevent loop if the plane is the source of the change
        SetSliceFromNormalizedValue(sliderValue, false);

        //if (!dataProvider.IsDataReady) return;
        
        //float targetZ = Mathf.Lerp(minZ, maxZ, sliderValue);
        
        //SlicedData = dataProvider.DataPoints.AsParallel().Where(point => 
        //    Mathf.Abs(point.position.z - targetZ) <= sliceThickness / 2f
        //).ToList();
        
        //OnSliceUpdated?.Invoke();
    }

    /// <summary>
    /// This is the master function that updates the slice position.
    /// It can be called by the UI slider or the draggable plane.
    /// </summary>
    /// <param name="normalizedValue">The new slice position, from 0.0 to 1.0.</param>
    /// <param name="calledFromPlane">True if this update was initiated by the physical plane.</param>
    public void SetSliceFromNormalizedValue(float normalizedValue, bool calledFromPlane)
    {
        if (!dataProvider.IsDataReady) return;

        // Update the UI Slider if the change came from the plane
        if (calledFromPlane)
        {
            isUpdatingFromPlane = true;
            positionSlider.value = normalizedValue;
            isUpdatingFromPlane = false;
        }
        
        float targetZ = Mathf.Lerp(minZ, maxZ, normalizedValue);

        // Update the physical plane's position
        if (slicerPlane != null)
        {
            slicerPlane.transform.position = new Vector3(
                slicerPlane.transform.position.x,
                slicerPlane.transform.position.y,
                targetZ
            );
        }
        
        // Filter the data for visualization
        SlicedData = dataProvider.DataPoints.AsParallel().Where(point => 
            Mathf.Abs(point.position.z - targetZ) <= sliceThickness / 2f
        ).ToList();
        
        OnSliceUpdated?.Invoke();
    }
}