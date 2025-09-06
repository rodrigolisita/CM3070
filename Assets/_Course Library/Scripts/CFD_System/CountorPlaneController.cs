using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// This script should be attached to the CFD_Visualizer object.
public class ContourPlaneController : MonoBehaviour
{
    [Header("Control")]
    [Tooltip("The UI Slider that controls the countor plane's position.")]
    public Slider controlSlider;
    [Tooltip("The Transform of the plane object to move.")]
    public Transform planeToMove;

    // --- Private Fields ---
    private CFD_DataSource dataSource;
//    private float minZ;
//    private float maxZ;

    void Awake()
    {
        // Automatically find the CFD_DataSource script on this same GameObject.
        dataSource = GetComponent<CFD_DataSource>();
        if (dataSource == null)
        {
            Debug.LogError("CFD_DataSource script not found on this GameObject!");
            this.enabled = false; // Disable if the source is missing.
            return;
        }
    }

    private void OnEnable()
    {
        dataSource.OnDataLoaded += OnDataReady;
        if (controlSlider != null)
        {
            controlSlider.onValueChanged.AddListener(UpdatePlanePosition);
        }
    }

    private void OnDisable()
    {
        dataSource.OnDataLoaded -= OnDataReady;
        if (controlSlider != null)
        {
            controlSlider.onValueChanged.RemoveListener(UpdatePlanePosition);
        }
    }

    // This is called when the CFD_DataSource finishes loading a file
    private void OnDataReady()
    {
        if (!dataSource.IsDataReady) return;

        // Set the slider to the middle and update the plane's position
        if (controlSlider != null)
        {
            controlSlider.value = 0.5f;
            UpdatePlanePosition(0.5f);
        }
    }

    // This public function is called by the slider's OnValueChanged event
    public void UpdatePlanePosition(float sliderValue)
    {
        if (planeToMove == null) return;

        // Use Mathf.Lerp to map the slider's 0-1 value to the data's Z-range
        float targetZ = Mathf.Lerp(dataSource.MinZ, dataSource.MaxZ, sliderValue);
//        float targetZ = Mathf.Lerp(minZ, maxZ, sliderValue);

        // Move the plane to the new Z position relative to its parent
        planeToMove.localPosition = new Vector3(
            planeToMove.localPosition.x,
            planeToMove.localPosition.y,
            targetZ
        );
    }
}