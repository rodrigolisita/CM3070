using UnityEngine;
using TMPro; // Don't forget to add this

public class PrefabTester : MonoBehaviour
{
    [Tooltip("Drag your TextEntryTemplate prefab here.")]
    public GameObject textEntryPrefab;

    [Tooltip("Drag the 'Content' object from your ScrollView here.")]
    public Transform contentParent;

    public void RunPrefabTest()
    {
        if (textEntryPrefab == null || contentParent == null)
    {
        Debug.LogError("Prefab or Content Parent not assigned!");
        return;
    }

    // 1. Create an instance of the prefab
    GameObject instance = Instantiate(textEntryPrefab, contentParent);

    // --- FIX STARTS HERE ---
    // 2. Get its RectTransform and reset its properties
    RectTransform rect = instance.GetComponent<RectTransform>();
    rect.localScale = Vector3.one; // Reset scale to (1, 1, 1)
    rect.localRotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
    rect.localPosition = Vector3.zero; // Reset position to (0, 0, 0)
    // --- FIX ENDS HERE ---

    // 3. Find the TextMeshPro component on the new instance
    TextMeshProUGUI textMesh = instance.GetComponentInChildren<TextMeshProUGUI>();

    // 4. Set its text directly
    if (textMesh != null)
    {
        textMesh.text = "Prefab instantiated and text set successfully.";
        Debug.Log("Test successful! Prefab was created and text was set.");
    }
    else
    {
        Debug.LogError("Test failed! Could not find a TextMeshProUGUI component on the instantiated prefab.");
    }
    }
}