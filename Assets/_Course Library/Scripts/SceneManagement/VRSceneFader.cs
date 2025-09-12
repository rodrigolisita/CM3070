using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class VRSceneFader : MonoBehaviour
{
    [Tooltip("The duration of the fade-in effect in seconds.")]
    public float fadeInDuration = 1.0f;

    private Material fadeMaterial;
    private Renderer fadeRenderer;

    void Awake()
    {
        // Get the renderer and create a unique instance of its material
        fadeRenderer = GetComponent<Renderer>();
        fadeMaterial = fadeRenderer.material;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Start fully opaque
        SetAlpha(1f);
        fadeRenderer.enabled = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            SetAlpha(newAlpha);
            yield return null;
        }

        // End fully transparent and disable the object so it doesn't use any resources
        SetAlpha(0f);
        fadeRenderer.enabled = false;
    }

    private void SetAlpha(float alpha)
    {
        Color currentColor = fadeMaterial.color;
        currentColor.a = alpha;
        fadeMaterial.color = currentColor;
    }
}