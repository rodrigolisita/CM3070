using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MassController : MonoBehaviour
{
    public ForceVisualizer forceVisualizer;

    // Reference to the Rigidbody component
    private Rigidbody rb;

    // Reference to the UI Text element to display the mass
    public TextMeshProUGUI massText;

    // Reference to the UI slider element to set the mass
    public Slider massSlider;

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
        
        // If Rigidbody is not found, log an error
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject!");
            return;
        }

        // Set the initial mass text
        UpdateMassText();

        // Update the slider's starting value to match the sphere's mass
        if (massSlider != null)
        {
            massSlider.value = rb.mass;
        }
    }

    // This is the public function the slider will call
    public void SetMass(float newMass)
    {
        // Make sure the Rigidbody was found before trying to use it
        if (rb != null)
        {
            rb.mass = newMass;
            UpdateMassText();
        }

        forceVisualizer.UpdateForceCalculations();
    }

    private void UpdateMassText()
    {
        if (massText != null && rb != null)
        {
            // Format the mass to one decimal place
            string formattedMass = rb.mass.ToString("F1");
            massText.text = $"{formattedMass} kg";
        }
    }
}