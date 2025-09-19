using UnityEngine;


/// <summary>
/// Change object active state
/// </summary>
public class ChangeObjectActiveState : MonoBehaviour
{
    [Tooltip("Time before destroying in seconds")]
    public float lifeTime = 5.0f;
    public GameObject go;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void InactivateGameObject()
    {
        go.SetActive(false);
    }
    
    public void ActivateGameObject()
    {
        go.SetActive(true);
    }
    
    
    
    
}
