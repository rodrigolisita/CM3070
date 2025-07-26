using UnityEngine;

public class playSoundCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    public AudioClip windowSound;
    private AudioSource objectAudio;
    private Rigidbody rb;
    public float maxExpectedMagnitude = 1.0f; // Adjust this value
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = rb.linearVelocity.magnitude;
        float normalizedMagnitude = Mathf.InverseLerp(0f, maxExpectedMagnitude, magnitude);

       // objectAudio.PlayOneShot(collisionSound, normalizedMagnitude);
        objectAudio.PlayOneShot(collisionSound, 1.0f);
       
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    float magnitude = rb.linearVelocity.magnitude;
    //    float normalizedMagnitude = Mathf.InverseLerp(0f, maxExpectedMagnitude, magnitude);
        
    //    if(other.CompareTag("Window"))
    //    {
    //        objectAudio.PlayOneShot(windowSound, normalizedMagnitude);
    //    }
    // }
}
