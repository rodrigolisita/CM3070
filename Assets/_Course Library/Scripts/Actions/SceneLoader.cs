using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple, reusable utility to load a specific scene.
/// The scene to load is configured in the Unity Inspector.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The exact name of the scene to load.")]
    [SerializeField] private string sceneNameToLoad;

    /// <summary>
    /// Loads the scene specified in the 'sceneNameToLoad' field.
    /// Assign this method to a button's OnClick event in the Inspector.
    /// </summary>
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError("Scene Name To Load is not set in the Inspector on this SceneLoader component.");
            return;
        }
        SceneManager.LoadScene(sceneNameToLoad);
    }
}