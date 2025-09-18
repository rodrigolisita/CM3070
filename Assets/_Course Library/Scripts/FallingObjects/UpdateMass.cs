using UnityEngine;
using TMPro;

public class UpdateMass : MonoBehaviour
{
    private Rigidbody rb;
    public float massIncrease = 1.0f;
    public TextMeshProUGUI massText; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMassText();        
    }

    public void increaseMass()
    {
        massIncrease ++;
        rb.mass +=0.1f;
        if(massIncrease > 10f)
        {
            massIncrease = 10.0f;
            rb.mass = 1f;
        }

        UpdateMassText();
        
        
        Debug.Log("Mass Increased! massIncrease: " + massIncrease);

    }

    public void decreateMass()
    {
        massIncrease --;
        rb.mass -=0.1f;
        if(massIncrease < 1f)
        {
            massIncrease = 1f;
            rb.mass = 0.1f;
        }

        UpdateMassText();
        
        
        Debug.Log("Mass Increased! massIncrease: " + massIncrease);

    }

    private void UpdateMassText(){
        
        //float mass = 100.0f*massIncrease;
        float mass = rb.mass;

        if (massText != null)
        {
            massText.text = mass.ToString("F2") + "[kg]"; // Format to 2 decimal places
        }
        else
        {
            Debug.LogError("massText is not assigned in the Inspector!");
        }
    }
}
