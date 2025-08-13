using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CFD_DataSource))]
public class DataSlicer : MonoBehaviour
{
    [Header("Slicing Control")]
    public Slider positionSlider;
    public Slider thicknessSlider; // Slider for thickness
    [Tooltip("The thickness of the slice to be displayed.")]
    public float sliceThickness = 0.1f;

    public List<CFD_DataSource.DataPoint> SlicedData { get; private set; } = new List<CFD_DataSource.DataPoint>();
    public event System.Action OnSliceUpdated;
    
    private CFD_DataSource dataSource;
    private float minZ, maxZ;

    void Awake()
    {
        dataSource = GetComponent<CFD_DataSource>();
    }

    private void OnEnable()
    {
        dataSource.OnDataLoaded += OnFullDataLoaded;
        if (positionSlider != null) positionSlider.onValueChanged.AddListener(OnSlicePositionChanged);
        if (thicknessSlider != null) thicknessSlider.onValueChanged.AddListener(OnSliceThicknessChanged);
    }

    private void OnDisable()
    {
        dataSource.OnDataLoaded -= OnFullDataLoaded;
        if (positionSlider != null) positionSlider.onValueChanged.RemoveListener(OnSlicePositionChanged);
        if (thicknessSlider != null) thicknessSlider.onValueChanged.RemoveListener(OnSliceThicknessChanged);
    }

    private void OnFullDataLoaded()
    {
        if (!dataSource.IsDataReady) return;
        minZ = dataSource.DataPoints.Min(p => p.position.z);
        maxZ = dataSource.DataPoints.Max(p => p.position.z);
        if (positionSlider != null)
        {
            positionSlider.value = 0.5f;
        }
        if (thicknessSlider != null)
        {
            thicknessSlider.value = 0.1f; // Set a small initial thickness
        }

        OnSlicePositionChanged(positionSlider.value);
    }
    
    // Function called by the thickness slider
    public void OnSliceThicknessChanged(float sliderValue)
    {
        // Map the slider's 0-1 value to a reasonable range, e.g., 0.001 to 0.1
        sliceThickness = Mathf.Lerp(0.001f, 0.2f, sliderValue);
        // Redraw the slice with the new thickness
        OnSlicePositionChanged(positionSlider.value);
    }

    public void OnSlicePositionChanged(float sliderValue)
    {
        if (!dataSource.IsDataReady) return;
        
        float targetZ = Mathf.Lerp(minZ, maxZ, sliderValue);
        
        SlicedData = dataSource.DataPoints.AsParallel().Where(point => 
            Mathf.Abs(point.position.z - targetZ) <= sliceThickness / 2f
        ).ToList();
        
        OnSliceUpdated?.Invoke();
    }
}