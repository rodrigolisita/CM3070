using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SphereSizeController : MonoBehaviour
{

    public ForceVisualizer forceVisualizer;

    // Reference to the UI Text element to display the scale
    public TextMeshProUGUI scaleText;
    // Reference to the UI Slider element to set the scale
    public Slider sizeSlider;

    // This is the public function the slider will call
    public void SetSize(float diameter)
    {
        // Set the object's scale uniformly based on the slider's value
        transform.localScale = Vector3.one * diameter;

        // Update the text display
        UpdateScaleText();

        forceVisualizer.UpdateForceCalculations();
    }

    // Updates the UI text with the current scale
    private void UpdateScaleText()
    {
        if (scaleText != null)
        {
            // We use the x-axis value, which is the diameter
            string formattedScale = transform.localScale.x.ToString("F1");
            scaleText.text = $"{formattedScale} m";
        }
    }

    void Start()
    {
        // Set the initial text when the scene starts
        UpdateScaleText();
        
        // Update the slider's starting value to match the sphere's scale
        if (sizeSlider != null)
        {
            sizeSlider.value = transform.localScale.x;
        }
    }
}