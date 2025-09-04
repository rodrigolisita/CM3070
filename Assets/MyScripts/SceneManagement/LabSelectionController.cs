using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene navigation for the Lab Selection screen.
/// </summary>
public class LabSelectionController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The exact name of the initial language selection scene.")]
    [SerializeField] private string selectionLanguageSceneName = "SelectionLanguage";

    /// <summary>
    /// A generic method to load any scene by its name.
    /// Used for buttons that will load the different laboratory scenes.
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not provided in the button's OnClick event.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// A specific method for the 'Back' button to return to the language selection screen.
    /// </summary>
    public void LoadLanguageSelectionScene()
    {
        SceneManager.LoadScene(selectionLanguageSceneName);
    }
}

