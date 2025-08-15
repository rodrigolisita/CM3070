using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DataSlicer))]
[RequireComponent(typeof(CFD_DataSource))]
public class GlyphVisualizer : MonoBehaviour
{
    [Header("Visualization Target")]
    public GameObject vectorGlyphPrefab;

    [Header("Visualization Settings")]
    public Gradient velocityGradient;
    [Tooltip("A multiplier for the size of the triangles.")]
    public float glyphScaleMultiplier = 0.04f;
    public bool useGlobalMinMax = false;
    public float globalMinVelocity = 0f;
    public float globalMaxVelocity = 2.0f;
    
    private CFD_DataSource mainDataSource;
    private List<GameObject> glyphPool = new List<GameObject>();
    private float prev_glyphScaleMultiplier;
    private DataSlicer dataSlicer;

    void Awake() 
    {
        mainDataSource = GetComponent<CFD_DataSource>();
        dataSlicer = GetComponent<DataSlicer>();
    }

    private void OnEnable() 
    {
        dataSlicer.OnSliceUpdated += DrawGlyphs;
    }

    private void OnDisable() 
    {
        dataSlicer.OnSliceUpdated -= DrawGlyphs;
    }

    void Update()
    {
        if (glyphScaleMultiplier != prev_glyphScaleMultiplier && glyphPool.Count > 0)
        {
            DrawGlyphs();
        }
    }

    public void DrawGlyphs()
    {
        if (vectorGlyphPrefab == null) return;

        prev_glyphScaleMultiplier = glyphScaleMultiplier;

        // Get the SLICED data to draw from the slicer
        List<CFD_DataSource.DataPoint> points = dataSlicer.SlicedData;

        // Use the min/max from the FULL dataset for consistent colors across all slices
        float minV = useGlobalMinMax ? globalMinVelocity : mainDataSource.MinVelocity;
        float maxV = useGlobalMinMax ? globalMaxVelocity : mainDataSource.MaxVelocity;
        float velocityRange = Mathf.Max(maxV - minV, 0.001f);

        // This loop operates on the smaller, sliced list
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            GameObject glyphInstance;
            if (i < glyphPool.Count) 
            {
                glyphInstance = glyphPool[i];
            }
            else 
            {
                glyphInstance = Instantiate(vectorGlyphPrefab, this.transform);
                glyphPool.Add(glyphInstance);
            }

            glyphInstance.SetActive(true);
            glyphInstance.transform.localPosition = p.position;
            float scaleFactor = glyphScaleMultiplier * p.velocityMagnitude;
            glyphInstance.transform.localScale = new Vector3(0.02f * scaleFactor, 0.08f * scaleFactor, 1f);
            
            Vector2 direction = new Vector2(p.velocityVector.x, p.velocityVector.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            glyphInstance.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            
            //SpriteRenderer[] spriteRenderers = glyphInstance.GetComponentsInChildren<SpriteRenderer>();
            // Calculate the color once
            //Color glyphColor = velocityGradient.Evaluate((p.velocityMagnitude - minV) / velocityRange);
            // Apply the same color to every sprite in the prefab
            //foreach (SpriteRenderer sr in spriteRenderers)
            //{
            //    sr.color = glyphColor;
            //}

            SpriteRenderer spriteRenderer = glyphInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) 
            {
                spriteRenderer.color = velocityGradient.Evaluate((p.velocityMagnitude - minV) / velocityRange);
            }
        }

        for (int i = points.Count; i < glyphPool.Count; i++) 
        {
            glyphPool[i].SetActive(false);
        }
    }

    public void ClearGlyphs()
    {
        foreach (var glyph in glyphPool)
        {
            glyph.SetActive(false);
        }
    }
}