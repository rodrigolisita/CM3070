using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class DataReader : MonoBehaviour
{
    // ... all the other code remains the same ...
    public GameObject pointPrefab;
    public Gradient velocityGradient;
    public List<string> simulationFilenames;
    [SerializeField] private TMP_Dropdown dropdown;

    private class DataPoint
    {
        public Vector3 position;
        public float velocity;
    }

    // This is the function the Dropdown calls
    public void OnSimulationSelected()
    {
        int index = dropdown.value;
        
        // --- NEW: Add this line to see what index we receive ---
        Debug.Log("Dropdown index received by script: " + index);

        // First, clear out any old points
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Check if the selected index is valid
        if (index < 0 || index >= simulationFilenames.Count)
        {
            Debug.LogError("Invalid dropdown index selected: " + index);
            return;
        }

        string selectedFile = simulationFilenames[index];
        LoadAndVisualizeData(selectedFile);
    }
    
    // ... The rest of the script (LoadAndVisualizeData, ParseContent, etc.) is exactly the same as before ...
    void LoadAndVisualizeData(string dataFileName)
    {
        if (pointPrefab == null) { Debug.LogError("Point Prefab is not assigned."); return; }
        string fileContent = ReadDataFile(dataFileName);
        if (string.IsNullOrEmpty(fileContent)) return;
        List<DataPoint> dataPoints = ParseContent(fileContent);
        if (dataPoints == null || dataPoints.Count == 0) { Debug.LogError("Failed to parse data."); return; }
        float minVelocity = dataPoints.Min(p => p.velocity);
        float maxVelocity = dataPoints.Max(p => p.velocity);
        InstantiateAndColorPoints(dataPoints, minVelocity, maxVelocity);
    }
    private string ReadDataFile(string dataFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        Debug.Log("Attempting to read data from: " + filePath);
        if (File.Exists(filePath))
        {
            try { return File.ReadAllText(filePath); }
            catch(System.Exception e) { Debug.LogError($"Failed to read file '{filePath}'. Error: {e.Message}"); return null; }
        }
        else { Debug.LogError($"File not found at path: '{filePath}'."); return null; }
    }
    private List<DataPoint> ParseContent(string content)
    {
        List<DataPoint> dataPoints = new List<DataPoint>();
        using (StringReader reader = new StringReader(content))
        {
            string line;
            bool isFirstLine = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine) { isFirstLine = false; continue; }
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] values = line.Split(',');
                if (values.Length < 5) continue;
                try
                {
                    CultureInfo culture = CultureInfo.InvariantCulture;
                    dataPoints.Add(new DataPoint {
                        position = new Vector3(float.Parse(values[1], culture), float.Parse(values[2], culture), float.Parse(values[3], culture)),
                        velocity = float.Parse(values[4], culture)
                    });
                }
                catch (System.Exception ex) { Debug.LogWarning($"Could not parse line: '{line}'. Error: {ex.Message}"); }
            }
        }
        return dataPoints;
    }
    void InstantiateAndColorPoints(List<DataPoint> points, float minVelocity, float maxVelocity)
    {
        if (this.transform.localScale != Vector3.one) { Debug.LogWarning("DataReaderObject scale is not (1,1,1)."); }
        float velocityRange = maxVelocity - minVelocity;
        if (velocityRange < float.Epsilon) velocityRange = 1f;
        foreach (var point in points)
        {
            GameObject instance = Instantiate(pointPrefab, this.transform);
            instance.transform.localPosition = point.position;
            float normalizedVelocity = (point.velocity - minVelocity) / velocityRange;
            Color pointColor = velocityGradient.Evaluate(normalizedVelocity);
            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null) { renderer.material.color = pointColor; }
        }
        Debug.Log($"Successfully instantiated {points.Count} points.");
    }
}