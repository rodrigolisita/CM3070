// Required namespaces for Unity functionality, UI, Coroutines, and Localization.
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// The central script for the physics simulation. It reads data from other controller scripts,
/// calculates forces (gravity, buoyancy, drag), and updates all UI text and visual arrows.
/// </summary>
public class ForceVisualizer : MonoBehaviour
{
    // --- REFERENCES TO OTHER SCENE COMPONENTS ---
    [Header("Component References")]
    public Rigidbody sphereRigidbody;      // The Rigidbody of the sphere to get its mass and scale.
    public FluidController fluidController;  // The script that manages fluid selection and density.

    [Header("UI Display")]
    public TextMeshProUGUI forcesText;       // The single text element for displaying all force data.

    // --- LOCALIZATION REFERENCES ---
    [Header("Localized Strings")]
    public LocalizedString gravityForceString;   // Key for the "Gravity Force" label.
    public LocalizedString buoyancyForceString;  // Key for the "Buoyancy Force" label.
    public LocalizedString dragForceString;      // Key for the "Drag Force" label.
    public LocalizedString terminalVelocityString; // Key for the "Terminal Velocity" label.
    public LocalizedString sinkingString;        // Key for the "Result: Sinking" message.
    public LocalizedString floatingString;       // Key for the "Result: Floating" message.
    public LocalizedString neutralString;        // Key for the "Result: Neutral Buoyancy" message.

    // --- SIMULATION PARAMETERS ---
    [Header("Physics Constants")]
    public float gravity = 9.81f;                // Acceleration due to gravity (m/s²).
    public float dragCoefficient = 0.47f;        // Standard drag coefficient for a sphere.

    [Header("Arrow Visuals")]
    public GameObject arrowModelPrefab;       // The 3D model/sprite prefab for the arrows.
    public Transform gravityArrowPivot;      // Empty object defining the position and parent for the gravity arrow.
    public Transform buoyancyArrowPivot;
    public Transform dragArrowPivot;
    public float maxArrowSize = 3f;              // The maximum visual size of the largest arrow in the scene.

    [Header("Animation")]
    public float animationDuration = 0.4f;       // How long the arrow scaling animation takes in seconds.
    
    [Header("Color Coding")]
    public Color gravityColor = Color.red;       // Color for gravity text and arrow.
    public Color buoyancyColor = Color.blue;     // Color for buoyancy text and arrow.
    public Color dragColor = Color.green;        // Color for drag text and arrow.

    // --- PRIVATE VARIABLES ---
    // References to the arrow GameObjects created at runtime.
    private Transform gravityArrowInstance;
    private Transform buoyancyArrowInstance;
    private Transform dragArrowInstance;
    // References to the running animation coroutines for each arrow.
    private Coroutine gravityAnim, buoyancyAnim, dragAnim;

    /// <summary>
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Instantiate the arrow visuals using a helper function.
        if (arrowModelPrefab != null)
        {
            gravityArrowInstance = CreateArrowInstance(gravityArrowPivot, gravityColor);
            buoyancyArrowInstance = CreateArrowInstance(buoyancyArrowPivot, buoyancyColor);
            dragArrowInstance = CreateArrowInstance(dragArrowPivot, dragColor);
        }

        // Subscribe to the language change event to update the UI text automatically.
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        // Perform an initial calculation and update to set the starting state.
        UpdateForceCalculations();
    }

    /// <summary>
    /// Helper function to reliably create an arrow instance, parent it, reset its transform, and color it.
    /// </summary>
    private Transform CreateArrowInstance(Transform pivot, Color arrowColor)
    {
        if (pivot == null) return null;

        // Create a new arrow from the prefab.
        GameObject newArrow = Instantiate(arrowModelPrefab);
        // Set its parent to the pivot object.
        newArrow.transform.SetParent(pivot);
        // Reset local transform properties to ensure it's perfectly aligned with the pivot.
        newArrow.transform.localPosition = Vector3.zero;
        newArrow.transform.localRotation = Quaternion.identity;
        newArrow.transform.localScale = Vector3.one;

        // Find all SpriteRenderers in the prefab's children and apply the specified color.
        SpriteRenderer[] spriteRenderers = newArrow.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            renderer.color = arrowColor;
        }

        return newArrow.transform;
    }

    /// <summary>
    /// This function is called whenever the selected language is changed.
    /// </summary>
    private void OnLocaleChanged(Locale obj)
    {
        // Recalculate and update the display to show the newly translated text.
        UpdateForceCalculations();
    }

    /// <summary>
    /// The main public function that recalculates all forces and updates the entire visualization.
    /// Called by the other controller scripts whenever a slider or dropdown value changes.
    /// </summary>
    public void UpdateForceCalculations()
    {
        if (sphereRigidbody == null || fluidController == null || forcesText == null) return;

        // --- 1. GET CURRENT VALUES from other components ---
        float mass = sphereRigidbody.mass;
        float diameter = sphereRigidbody.transform.localScale.x;
        float radius = diameter / 2f;
        float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
        float crossSectionalArea = Mathf.PI * Mathf.Pow(radius, 2);
        float fluidDensity = fluidController.CurrentFluidDensity;

        // --- 2. CALCULATE FORCES ---
        float gravityForce = mass * gravity;
        float buoyancyForce = fluidDensity * volume * gravity;
        // At terminal velocity, the drag force magnitude equals the net force between gravity and buoyancy.
        float netForce = Mathf.Abs(gravityForce - buoyancyForce);
        float dragForce = netForce; 
        // Calculate terminal velocity using the standard formula.
        float terminalVelocity = Mathf.Sqrt((2 * netForce) / (fluidDensity * crossSectionalArea * dragCoefficient));
        if (netForce < 0.001f) terminalVelocity = 0; // Handle case where forces are balanced.

        // --- 3. DETERMINE DIRECTION for the text display ---
        LocalizedString directionString;
        if (gravityForce > buoyancyForce + 0.01f) { directionString = sinkingString; }
        else if (buoyancyForce > gravityForce + 0.01f) { directionString = floatingString; }
        else { directionString = neutralString; }

        // --- 4. BUILD LOCALIZED & COLOR-CODED TEXT using TextMeshPro's Rich Text ---
        string gravityHex = ColorUtility.ToHtmlStringRGB(gravityColor);
        string buoyancyHex = ColorUtility.ToHtmlStringRGB(buoyancyColor);
        string dragHex = ColorUtility.ToHtmlStringRGB(dragColor);

        string displayText = $"<color=#{gravityHex}>{gravityForceString.GetLocalizedString()}: {gravityForce.ToString("F1")} N</color>\n" +
                             $"<color=#{buoyancyHex}>{buoyancyForceString.GetLocalizedString()}: {buoyancyForce.ToString("F1")} N</color>\n" +
                             $"<color=#{dragHex}>{dragForceString.GetLocalizedString()}: {dragForce.ToString("F1")} N</color>\n" +
                             $"{terminalVelocityString.GetLocalizedString()}: {terminalVelocity.ToString("F1")} m/s\n\n" +
                             $"{directionString.GetLocalizedString()}";
        forcesText.text = displayText;

        // --- 5. START ARROW ANIMATIONS ---
        // Normalize the forces to determine the relative size of each arrow.
        float maxForce = Mathf.Max(gravityForce, buoyancyForce, dragForce);
        if (maxForce > 0.001f)
        {
            // Calculate the target height for each arrow based on the normalization.
            float gravHeight = (gravityForce / maxForce) * maxArrowSize;
            float buoyHeight = (buoyancyForce / maxForce) * maxArrowSize;
            float dragHeight = (dragForce / maxForce) * maxArrowSize;

            // Stop any previous animations and start new ones to the new target sizes.
            if (gravityArrowInstance) { if (gravityAnim != null) StopCoroutine(gravityAnim); gravityAnim = StartCoroutine(AnimateArrowScale(gravityArrowInstance, gravHeight)); }
            if (buoyancyArrowInstance) { if (buoyancyAnim != null) StopCoroutine(buoyancyAnim); buoyancyAnim = StartCoroutine(AnimateArrowScale(buoyancyArrowInstance, buoyHeight)); }
            if (dragArrowInstance) { if (dragAnim != null) StopCoroutine(dragAnim); dragAnim = StartCoroutine(AnimateArrowScale(dragArrowInstance, dragHeight)); }
        }
        else // If all forces are zero, instantly set scales to a minimal value.
        {
             if(gravityArrowInstance) gravityArrowInstance.localScale = new Vector3(1, 0, 1);
             if(buoyancyArrowInstance) buoyancyArrowInstance.localScale = new Vector3(1, 0, 1);
             if(dragArrowInstance) dragArrowInstance.localScale = new Vector3(1, 0, 1);
        }
        
        // Instantly set the rotation of the arrows.
        if (gravityArrowInstance) gravityArrowInstance.localRotation = Quaternion.Euler(0, 0, 180); // Point down
        if (buoyancyArrowInstance) buoyancyArrowInstance.localRotation = Quaternion.identity; // Point up
        if (dragArrowInstance) dragArrowInstance.localRotation = (gravityForce > buoyancyForce) ? Quaternion.identity : Quaternion.Euler(0, 0, 180); // Opposes net force
    }

    /// <summary>
    /// A Coroutine that smoothly animates an arrow's scale from its current height to a target height over a set duration.
    /// </summary>
    private IEnumerator AnimateArrowScale(Transform arrow, float targetHeight)
    {
        Vector3 startScale = arrow.localScale;
        Vector3 targetScale = new Vector3(startScale.x, targetHeight, startScale.z);
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            // Use Lerp (Linear Interpolation) to find the scale value at the current point in time.
            arrow.localScale = Vector3.Lerp(startScale, targetScale, timeElapsed / animationDuration);
            // Increment the timer by the time that has passed since the last frame.
            timeElapsed += Time.deltaTime;
            // Pause the function until the next frame.
            yield return null;
        }

        // After the loop, snap to the final scale to ensure it's perfectly accurate.
        arrow.localScale = targetScale;
    }
}