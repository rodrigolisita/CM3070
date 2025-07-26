using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// This script requires a CFD_DataSource to be on the same GameObject.
[RequireComponent(typeof(CFD_DataSource))]
public class ParticleSystemVisualizer : MonoBehaviour
{
    [Header("Visualization Target")]
    [Tooltip("The Particle System that will display the data.")]
    public ParticleSystem dataParticleSystem;

    [Header("Visualization Settings")]
    [Tooltip("Color gradient for velocity magnitude.")]
    public Gradient velocityGradient;
    [Tooltip("The base size for each particle (or the width of the vector lines).")]
    public float particleSize = 0.005f;
    [Tooltip("Multiplier for the length of the vector lines.")]
    public float vectorLengthMultiplier = 0.01f;
    [Tooltip("Normalize colors against a fixed global range instead of the file's own range.")]
    public bool useGlobalMinMax = false;
    [Tooltip("The global minimum velocity for the color gradient.")]
    public float globalMinVelocity = 0f;
    [Tooltip("The global maximum velocity for the color gradient.")]
    public float globalMaxVelocity = 2.0f;

    // --- Private Fields ---
    private CFD_DataSource dataSource;
    private bool showAsVectors = true; // Default to showing vectors
    private ParticleSystem.Particle[] particles;

    void Awake()
    {
        dataSource = GetComponent<CFD_DataSource>();
    }

    private void OnEnable()
    {
        dataSource.OnDataLoaded += UpdateParticleVisuals;
    }

    private void OnDisable()
    {
        dataSource.OnDataLoaded -= UpdateParticleVisuals;
    }

    // This is the public function the UI Toggle calls
    public void OnViewModeChanged(bool showVectors)
    {
        this.showAsVectors = showVectors;
        if (dataSource.IsDataReady)
        {
            UpdateParticleVisuals();
        }
    }

    public void UpdateParticleVisuals()
    {
        if (!dataSource.IsDataReady || dataParticleSystem == null)
        {
            if (dataParticleSystem != null) dataParticleSystem.Clear();
            return;
        }
        
        List<CFD_DataSource.DataPoint> points = dataSource.DataPoints;
        float minV = useGlobalMinMax ? globalMinVelocity : dataSource.MinVelocity;
        float maxV = useGlobalMinMax ? globalMaxVelocity : dataSource.MaxVelocity;
        float velocityRange = maxV - minV;
        if (velocityRange < float.Epsilon) velocityRange = 1f;
        
        // In vector mode, we need two particles for each line.
        int requiredParticles = showAsVectors ? points.Count * 2 : points.Count;
        
        if (particles == null || particles.Length < requiredParticles)
        {
            particles = new ParticleSystem.Particle[requiredParticles];
        }

        var main = dataParticleSystem.main;
        if (main.maxParticles < requiredParticles)
        {
            main.maxParticles = requiredParticles;
        }

        if (showAsVectors)
        {
            int pIndex = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                Color c = velocityGradient.Evaluate((p.velocityMagnitude - minV) / velocityRange);

                // Particle 1: The start of the vector line
                particles[pIndex].position = p.position;
                particles[pIndex].startColor = c;
                particles[pIndex].startSize = particleSize;

                // Particle 2: The end of the vector line
                particles[pIndex + 1].position = p.position + p.velocityVector * vectorLengthMultiplier;
                particles[pIndex + 1].startColor = c;
                particles[pIndex + 1].startSize = particleSize;

                pIndex += 2;
            }
        }
        else // Show as points (scalar magnitude)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                particles[i].position = p.position;
                particles[i].startColor = velocityGradient.Evaluate((p.velocityMagnitude - minV) / velocityRange);
                particles[i].startSize = particleSize;
            }
        }
        
        dataParticleSystem.SetParticles(particles, requiredParticles);

        // Configure the trails module to draw the lines between particle pairs for vectors
        var trails = dataParticleSystem.trails;
        trails.enabled = showAsVectors;
        if (showAsVectors)
        {
            trails.mode = ParticleSystemTrailMode.PerParticle;
            trails.ratio = 1f;
            trails.lifetime = new ParticleSystem.MinMaxCurve(100000f); // Make trails effectively permanent
        }
    }
}