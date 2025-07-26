using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CFD_DataSource))]
public class VectorPlotVisualizer : MonoBehaviour
{
    [Header("Visualization Target")]
    public ParticleSystem dataParticleSystem;

    [Header("Visualization Settings")]
    public Gradient velocityGradient;
    public float vectorWidth = 0.005f;
    public float vectorLengthMultiplier = 0.5f;
    public bool useGlobalMinMax = false;
    public float globalMinVelocity = 0f;
    public float globalMaxVelocity = 2.0f;

    private CFD_DataSource dataSource;
    private ParticleSystem.Particle[] particles;

    void Awake()
    {
        dataSource = GetComponent<CFD_DataSource>();
    }

    private void OnEnable()
    {
        dataSource.OnDataLoaded += DrawVectors;
    }

    private void OnDisable()
    {
        dataSource.OnDataLoaded -= DrawVectors;
    }

    void Update()
    {
        // Redraw if a value is changed in the Inspector
        if (transform.hasChanged)
        {
            if (dataSource.IsDataReady) DrawVectors();
            transform.hasChanged = false;
        }
    }

    public void DrawVectors()
    {
        if (!dataSource.IsDataReady || dataParticleSystem == null) return;
        
        List<CFD_DataSource.DataPoint> points = dataSource.DataPoints;
        float minV = useGlobalMinMax ? globalMinVelocity : dataSource.MinVelocity;
        float maxV = useGlobalMinMax ? globalMaxVelocity : dataSource.MaxVelocity;
        float velocityRange = maxV - minV;
        if (velocityRange < float.Epsilon) velocityRange = 1f;
        
        int requiredParticles = points.Count * 2;
        if (particles == null || particles.Length < requiredParticles)
        {
            particles = new ParticleSystem.Particle[requiredParticles];
        }

        var main = dataParticleSystem.main;
        if (main.maxParticles < requiredParticles)
        {
            main.maxParticles = requiredParticles;
        }

        int pIndex = 0;
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            Color c = velocityGradient.Evaluate((p.velocityMagnitude - minV) / velocityRange);
            Vector3 scaledVector = p.velocityVector.normalized * p.velocityMagnitude * vectorLengthMultiplier;
            
            particles[pIndex].position = p.position;
            particles[pIndex].startColor = c;
            particles[pIndex].startSize = vectorWidth;

            particles[pIndex + 1].position = p.position + scaledVector;
            particles[pIndex + 1].startColor = c;
            particles[pIndex + 1].startSize = vectorWidth;

            pIndex += 2;
        }
        
        dataParticleSystem.SetParticles(particles, requiredParticles);
    }
}