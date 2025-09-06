using UnityEngine;
using UnityEngine.UI;

public class FollowCameraHeight : MonoBehaviour
{
    [Header("Camera")]
    public Transform vrCamera;

    [Header("UI Control")]
    public Slider heightSlider;

    void Start()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera is not assigned!");
            this.enabled = false;
            return;
        }
        
        // Set the slider to the middle (no adjustment) when the scene starts
        if (heightSlider != null)
        {
            heightSlider.value = 0.5f;
        }
    }

    // Update is called every frame
    void Update()
    {
        // Calculate the user's desired offset from the slider
        // This maps the slider to a range, e.g., -0.5m to +0.5m from eye level
        float sliderAdjustment = (heightSlider != null) ? (heightSlider.value - 0.5f) * 1.0f : 0f;

        // Set the object's final Y position
        transform.position = new Vector3(
            transform.position.x,
            // Start at the camera's current height and add the user's slider adjustment
            vrCamera.position.y + sliderAdjustment,
            transform.position.z
        );
    }
}