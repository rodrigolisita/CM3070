using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(DataSlicer))]
[RequireComponent(typeof(CFD_DataSource))]
public class ContourPlotVisualizer : MonoBehaviour
{
    [Header("Visualization Target")]
    public Renderer targetRenderer;

    [Header("Visualization Settings")]
    public Gradient velocityGradient;
    public int textureResolution = 128;
    public bool useGlobalMinMax = false;
    public float globalMinVelocity = 0f;
    public float globalMaxVelocity = 2.0f;

    private DataSlicer dataSlicer;
    private CFD_DataSource dataSource;
    private Texture2D contourTexture;

    void Awake()
    {
        dataSlicer = GetComponent<DataSlicer>();
        dataSource = GetComponent<CFD_DataSource>();
        contourTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        contourTexture.wrapMode = TextureWrapMode.Clamp;
        contourTexture.filterMode = FilterMode.Bilinear;

        if (targetRenderer != null)
            targetRenderer.material.mainTexture = contourTexture;
    }

    //void OnEnable() => dataSlicer.OnSliceUpdated += GenerateContour;
    void OnEnable()
    {
        dataSlicer.OnSliceUpdated += GenerateContour;
        
        // Force an initial draw when the script is enabled,
        // in case we missed the first event from the slicer.
        GenerateContour();
    }
    void OnDisable() => dataSlicer.OnSliceUpdated -= GenerateContour;

    public void GenerateContour()
    {
        if (!dataSource.IsDataReady || targetRenderer == null)
            return;

        List<CFD_DataSource.DataPoint> points = dataSlicer.SlicedData;
        if (points.Count == 0)
        {
            // If the slice is empty, create a transparent texture
            Color[] clearColors = new Color[textureResolution * textureResolution];
            for (int i = 0; i < clearColors.Length; i++) { clearColors[i] = Color.clear; }
            contourTexture.SetPixels(clearColors);
            //contourTexture.SetPixels(new Color[textureResolution * textureResolution]);
            contourTexture.Apply();
            return;
        }

        float minV = useGlobalMinMax ? globalMinVelocity : dataSource.MinVelocity;
        float maxV = useGlobalMinMax ? globalMaxVelocity : dataSource.MaxVelocity;
        float velocityRange = Mathf.Max(maxV - minV, 0.001f);

        float minX = points.Min(p => p.position.x);
        float maxX = points.Max(p => p.position.x);
        float minY = points.Min(p => p.position.y);
        float maxY = points.Max(p => p.position.y);
        float zPos = points[0].position.z;

        float cellSizeX = (maxX - minX) / textureResolution;
        float cellSizeY = (maxY - minY) / textureResolution;

        Dictionary<(int, int), List<CFD_DataSource.DataPoint>> gridMap = new();
        foreach (var point in points)
        {
            int gx = Mathf.FloorToInt((point.position.x - minX) / cellSizeX);
            int gy = Mathf.FloorToInt((point.position.y - minY) / cellSizeY);
            var key = (gx, gy);

            if (!gridMap.TryGetValue(key, out var bin))
            {
                bin = new List<CFD_DataSource.DataPoint>();
                gridMap[key] = bin;
            }
            bin.Add(point);
        }

        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, zPos);
        float width = maxX - minX;
        float height = maxY - minY;

        targetRenderer.transform.localScale = new Vector3(width, height, 1f);
        Vector3 offset = new Vector3(width / textureResolution, 0f, 0f);
        targetRenderer.transform.localPosition = center + offset;

        Color[] pixelColors = new Color[textureResolution * textureResolution];

        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                float px = Mathf.Lerp(minX, maxX, x / (float)(textureResolution - 1));
                float py = Mathf.Lerp(minY, maxY, y / (float)(textureResolution - 1));
                int gx = Mathf.FloorToInt((px - minX) / cellSizeX);
                int gy = Mathf.FloorToInt((py - minY) / cellSizeY);
                Vector3 pixelWorldPos = new Vector3(px, py, zPos);

                List<CFD_DataSource.DataPoint> candidates = new();
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (gridMap.TryGetValue((gx + dx, gy + dy), out var list))
                            candidates.AddRange(list);

                if (candidates.Count == 0)
                    for (int dx = -2; dx <= 2; dx++)
                        for (int dy = -2; dy <= 2; dy++)
                            if (gridMap.TryGetValue((gx + dx, gy + dy), out var list))
                                candidates.AddRange(list);

                Vector3 weightedVelocity = Vector3.zero;
                float totalWeight = 0f;

                foreach (var p in candidates)
                {
                    float dist = Vector3.Distance(pixelWorldPos, p.position);
                    float weight = 1f / Mathf.Max(dist, 0.001f);
                    weightedVelocity += p.velocityVector * weight;
                    totalWeight += weight;
                }

                if (totalWeight > 0f)
                    weightedVelocity /= totalWeight;

                float magnitude = weightedVelocity.magnitude;
                float normalized = Mathf.Clamp01((magnitude - minV) / velocityRange);

                int flippedX = textureResolution - 1 - x;
                int flippedY = y;
                pixelColors[flippedY * textureResolution + flippedX] = velocityGradient.Evaluate(normalized);
            }
        }

        contourTexture.SetPixels(pixelColors);
        contourTexture.Apply();
    }
}
