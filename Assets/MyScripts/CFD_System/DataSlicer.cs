using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CFD_DataSource))]
public class DataSlicer : MonoBehaviour
{
    [Header("Slicing Control")]
    public Slider sliceSlider;
    [Tooltip("The thickness of the slice to be displayed.")]
    public float sliceThickness = 0.01f;

    // Public properties that the visualizer script will access
    public List<CFD_DataSource.DataPoint> SlicedData { get; private set; } = new List<CFD_DataSource.DataPoint>();
    public event System.Action OnSliceUpdated;
    
    private CFD_DataSource dataSource;
    private float minZ, maxZ; // To store the Z-range of the data

    void Awake()
    {
        dataSource = GetComponent<CFD_DataSource>();
    }

    private void OnEnable()
    {
        dataSource.OnDataLoaded += OnFullDataLoaded;
    }

    private void OnDisable()
    {
        dataSource.OnDataLoaded -= OnFullDataLoaded;
    }

    // This is called once when a new file is loaded
    private void OnFullDataLoaded()
    {
        if (!dataSource.IsDataReady) return;

        // Find the Z-range of the new dataset
        minZ = dataSource.DataPoints.Min(p => p.position.z);
        maxZ = dataSource.DataPoints.Max(p => p.position.z);

        // Update the slider to match the new data
        sliceSlider.value = 0.5f; // Start in the middle
        OnSlicePositionChanged(sliceSlider.value);
    }

    // This is the public function called by the slider's event
    public void OnSlicePositionChanged(float sliderValue) // value is 0-1
    {
        if (!dataSource.IsDataReady) return;

        // Convert the slider's 0-1 value to a world Z-coordinate
        float targetZ = Mathf.Lerp(minZ, maxZ, sliderValue);

        // Filter the full dataset to get only the points within the slice
        SlicedData = dataSource.DataPoints.AsParallel().Where(point => 
            Mathf.Abs(point.position.z - targetZ) <= sliceThickness / 2f
        ).ToList();
        
        // Announce that the new slice is ready for visualization
        OnSliceUpdated?.Invoke();
    }
}