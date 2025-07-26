using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    public float gravity;
    public float fluidDensity; // Air density (kg/m³)
    public float fluidBuoyancyMultiplier = 1.0f; // adjust to simulate buoyancy.

    private Rigidbody rb;
    private float objectCrossSectionalArea;
    private float objectVolume;

    private float dragCoefficient;
    public float sphereDragCoefficient = 0.47f; // Sphere drag coefficient
    public float cubeDragCoefficient = 1.05f; // Cube drag coefficient

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CalculateProperties(); 
    }

    void FixedUpdate()
    {
               
        // Gravitational force
        Vector3 gravityForce = Vector3.down * rb.mass * gravity;
        rb.AddForce(gravityForce);

        // Buoyant force
        Vector3 buoyancyForce = Vector3.up * fluidDensity * objectVolume * gravity * fluidBuoyancyMultiplier;
        rb.AddForce(buoyancyForce);

        // Drag force
        Vector3 dragForce = -rb.linearVelocity.normalized * 0.5f * fluidDensity * rb.linearVelocity.sqrMagnitude * dragCoefficient * objectCrossSectionalArea;
        rb.AddForce(dragForce);
    }

    void CalculateProperties() // Combined calculation
    {
        if (gameObject.CompareTag("Sphere"))
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                float radius = sphereCollider.radius * transform.localScale.x;
                objectCrossSectionalArea = Mathf.PI * Mathf.Pow(radius, 2);
                objectVolume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3f);
                dragCoefficient = sphereDragCoefficient;
                Debug.Log("Sphere Cross-sectional area: " + objectCrossSectionalArea);
                Debug.Log("Sphere Volume: " + objectVolume);
                //Debug.Log("Sphere Drag Coefficient: " + dragCoefficient);
            }
            else
            {
                Debug.LogError("Sphere Collider not found!");
            }
        }
        else if (gameObject.CompareTag("Cube"))
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                float sideLength = boxCollider.size.x * transform.localScale.x;
                //objectCrossSectionalArea = Mathf.Pow(sideLength, 2);
                //objectVolume = Mathf.Pow(sideLength, 3f);
                objectCrossSectionalArea = (boxCollider.size.x * transform.localScale.x) * (boxCollider.size.z * transform.localScale.z);
                objectVolume = (boxCollider.size.x * transform.localScale.x) * (boxCollider.size.y * transform.localScale.y) * (boxCollider.size.z * transform.localScale.z);
    
                dragCoefficient = cubeDragCoefficient;
                Debug.Log("Cube Cross-sectional area: " + objectCrossSectionalArea);
                Debug.Log("Cube Volume: " + objectVolume);
                Debug.Log("Cube Drag Coefficient: " + dragCoefficient);
            }
            else
            {
                Debug.LogError("Box Collider not found!");
            }
        }
        else
        {
            Debug.LogError("Object tag not recognized!");
        }
    }

  
}
