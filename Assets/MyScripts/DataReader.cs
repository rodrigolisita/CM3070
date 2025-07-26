using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class DataReader : MonoBehaviour
{
    [Header("Visualization Target")]
    [Tooltip("The Particle System that will display the data.")]
    public ParticleSystem dataParticleSystem;

    [Header("Visualization Settings")]
    [Tooltip("Color gradient for velocity magnitude.")]
    public Gradient velocityGradient;
    [Tooltip("The base size for each particle.")]
    public float particleSize = 0.01f;
    [Tooltip("Normalize colors against a fixed global range instead of the file's own range.")]
    public bool useGlobalMinMax = false;
    [Tooltip("The global minimum velocity for the color gradient.")]
    public float globalMinVelocity = 0f;
    [Tooltip("The global maximum velocity for the color gradient.")]
    public float globalMaxVelocity = 2.0f;

    [Header("Data Source")]
    [Tooltip("UI Dropdown for simulation selection.")]
    public TMP_Dropdown dropdown;
    [Tooltip("List of data files in StreamingAssets.")]
    public List<string> simulationFilenames;

    // --- Private Fields ---
    private List<DataPoint> currentDataPoints = new List<DataPoint>();
    private ParticleSystem.Particle[] particles;

    // --- DataPoint holds all data ---
    private class DataPoint
    {
        public Vector3 position;
        public Vector3 velocityVector;
        public float velocityMagnitude;
    }

    // --- UI Event Handlers ---
    public void OnSimulationSelected()
    {
        if (dropdown == null) return;
        LoadDataAndVisualize(dropdown.value);
    }

    void Start()
    {
        if (simulationFilenames.Count > 0)
        {
            LoadDataAndVisualize(0);
        }
    }
    
    // --- Core Logic ---
    void LoadDataAndVisualize(int index)
    {
        if (index < 0 || index >= simulationFilenames.Count) return;
        string selectedFile = simulationFilenames[index];
        string fileContent = ReadDataFile(selectedFile);
        if (string.IsNullOrEmpty(fileContent)) return;
        currentDataPoints = ParseContent(fileContent);
        if (currentDataPoints.Count == 0) { Debug.LogError("No data points parsed."); return; }
        UpdateParticleVisuals();
    }
    
    void UpdateParticleVisuals()
    {
        if (currentDataPoints.Count == 0 || dataParticleSystem == null) return;
        float minV, maxV;
        if (useGlobalMinMax)
        {
            minV = globalMinVelocity;
            maxV = globalMaxVelocity;
        }
        else
        {
            minV = currentDataPoints.Min(p => p.velocityMagnitude);
            maxV = currentDataPoints.Max(p => p.velocityMagnitude);
        }
        float velocityRange = maxV - minV;
        if (velocityRange < float.Epsilon) velocityRange = 1f;
        if (particles == null || particles.Length < currentDataPoints.Count)
        {
            particles = new ParticleSystem.Particle[currentDataPoints.Count];
        }
        var main = dataParticleSystem.main;
        if (main.maxParticles < currentDataPoints.Count)
        {
            main.maxParticles = currentDataPoints.Count;
        }
        for (int i = 0; i < currentDataPoints.Count; i++)
        {
            DataPoint point = currentDataPoints[i];
            particles[i].position = point.position;
            float normalizedValue = (point.velocityMagnitude - minV) / velocityRange;
            particles[i].startColor = velocityGradient.Evaluate(normalizedValue);
            particles[i].startSize = particleSize;
        }
        dataParticleSystem.SetParticles(particles, currentDataPoints.Count);
        var trails = dataParticleSystem.trails;
        trails.enabled = false;
    }
    
    // --- Helper Functions ---
    private string ReadDataFile(string dataFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        if (File.Exists(filePath))
        {
            try { return File.ReadAllText(filePath); }
            catch(System.Exception e) { Debug.LogError($"Read error: {e.Message}"); return null; }
        } else { Debug.LogError($"File not found: {filePath}"); return null; }
    }

    // --- ParseContent reads all 8 columns ---
    private List<DataPoint> ParseContent(string content)
    {
        var dataPoints = new List<DataPoint>();
        using (var reader = new StringReader(content))
        {
            string line = reader.ReadLine(); // Skip header
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] values = line.Split(',');
                // Check for all 8 columns
                if (values.Length < 8) continue; 
                try
                {
                    var culture = CultureInfo.InvariantCulture;
                    dataPoints.Add(new DataPoint
                    {
                        position = new Vector3(float.Parse(values[1], culture), float.Parse(values[2], culture), float.Parse(values[3], culture)),
                        velocityMagnitude = float.Parse(values[4], culture),
                        velocityVector = new Vector3(float.Parse(values[5], culture), float.Parse(values[6], culture), float.Parse(values[7], culture))
                    });
                }
                catch (System.Exception ex) { Debug.LogWarning($"Could not parse line: '{line}'. Error: {ex.Message}"); }
            }
        }
        return dataPoints;
    }
}