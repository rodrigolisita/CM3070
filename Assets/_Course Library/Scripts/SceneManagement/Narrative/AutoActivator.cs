using UnityEngine;

public class AutoActivator : MonoBehaviour
{
    [Tooltip("If checked, the objects below will be activated when the scene starts.")]
    public bool startAutomatically = false;

    [Tooltip("Drag all the GameObjects you want to activate here (e.g., your Interface and SceneManager).")]
    public GameObject[] objectsToActivate;

    void Start()
    {
        if (startAutomatically)
        {
            ActivateObjects();
        }
    }

    // You can also call this function from a button's OnClick event
    public void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}