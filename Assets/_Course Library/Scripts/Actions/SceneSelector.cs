using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // Add this for TextMeshPro Dropdowns

public class SceneSelector : MonoBehaviour
{
    // Make sure to drag your Dropdown UI element here in the Inspector
    public TMP_Dropdown sceneDropdown; 
    public List<string> sceneNames;

    void Start()
    {
        // When the scene starts, sync the dropdown to the current scene
        SyncDropdownToCurrentScene();
    }

    void SyncDropdownToCurrentScene()
    {
        if (sceneDropdown == null) return;

        // Get the name of the currently active scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Find the index of the current scene in our list of names
        int sceneIndex = sceneNames.IndexOf(currentSceneName);

        // If we found a match, set the dropdown's value to that index
        // The 'false' at the end tells the dropdown NOT to fire the OnValueChanged event
        if (sceneIndex != -1)
        {
            sceneDropdown.SetValueWithoutNotify(sceneIndex);
        }
    }

    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < sceneNames.Count)
        {
            string sceneToLoad = sceneNames[index];
            Debug.Log("Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);

        }
        else
        {
            Debug.LogWarning("Invalid scene index: " + index);
        }
    }

    public void LoadNextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = sceneNames.IndexOf(currentSceneName);

        // Use the modulo operator to wrap around to the beginning.
        int nextSceneIndex = (currentSceneIndex + 1) % sceneNames.Count;
        
        SceneManager.LoadScene(sceneNames[nextSceneIndex]);
    }

    public void LoadPreviousScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = sceneNames.IndexOf(currentSceneName);

        // If we are at the first scene (index 0), go to the last one.
        int previousSceneIndex = currentSceneIndex - 1;
        if (previousSceneIndex < 0)
        {
            previousSceneIndex = sceneNames.Count - 1;
        }

        SceneManager.LoadScene(sceneNames[previousSceneIndex]);
    }
}