using UnityEngine;
using TMPro;

public class FallTimer : MonoBehaviour
{
    private Rigidbody rb;
    private float startTime;
    private bool isFalling = false;
    private float fallTime = 0f;

    public bool displayTime = true; // Add this line
    [SerializeField] private TextMeshProUGUI timeText; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeText();  
    }

     public void StartFalling()
    {
        if (!isFalling)
        {
            startTime = Time.time;
            isFalling = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isFalling)
        {
            fallTime = Time.time - startTime;
            isFalling = false;
            Debug.Log("Fall time: " + fallTime + " seconds");
        }
    }

    private void UpdateTimeText()
    {
        if (displayTime && timeText != null) // Check if displayTime is true
        {
            timeText.text = fallTime.ToString("F3") + "[s]";
        }
    }

}
