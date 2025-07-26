using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

public class CFD_DataSource : MonoBehaviour
{
    [Header("Data Source")]
    public TMP_Dropdown dropdown;
    public List<string> simulationFilenames;

    [Header("Data Processing")]
    public bool force2DProjection = false;

    public List<DataPoint> DataPoints { get; private set; } = new List<DataPoint>();
    public float MinVelocity { get; private set; } = 0f;
    public float MaxVelocity { get; private set; } = 1f;
    public bool IsDataReady { get; private set; } = false;
    
    public event System.Action OnDataLoaded;

    public class DataPoint
    {
        public Vector3 position;
        public Vector3 velocityVector;
        public float velocityMagnitude;
    }

    void Start()
    {
        if (simulationFilenames.Count > 0) { LoadDataFileByIndex(0); }
    }

    public void OnSimulationSelected()
    {
        if (dropdown == null) return;
        LoadDataFileByIndex(dropdown.value);
    }
    
    private void LoadDataFileByIndex(int index)
    {
        if (index < 0 || index >= simulationFilenames.Count) return;
        string selectedFile = simulationFilenames[index];
        string fileContent = ReadDataFile(selectedFile);
        if (string.IsNullOrEmpty(fileContent)) { IsDataReady = false; return; }
        DataPoints = ParseContent(fileContent);
        if (DataPoints.Count == 0) { IsDataReady = false; Debug.LogError("No data points parsed."); return; }
        MinVelocity = DataPoints.Min(p => p.velocityMagnitude);
        MaxVelocity = DataPoints.Max(p => p.velocityMagnitude);
        IsDataReady = true;
        OnDataLoaded?.Invoke();
    }
    
    private List<DataPoint> ParseContent(string content)
    {
        var dataPoints = new List<DataPoint>();
        using (var reader = new StringReader(content))
        {
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] values = line.Split(',');
                if (values.Length < 8) continue; 
                try
                {
                    var culture = CultureInfo.InvariantCulture; // Ensures the decimal separator is always a dot
                    float x_pos = float.Parse(values[1], culture);
                    float y_pos = float.Parse(values[2], culture);
                    float z_pos = float.Parse(values[3], culture);
                    float vx = float.Parse(values[5], culture);
                    float vy = float.Parse(values[6], culture);
                    float vz = float.Parse(values[7], culture);

                    if (force2DProjection) { z_pos = 0f; vz = 0f; }

                    dataPoints.Add(new DataPoint
                    {
                        position = new Vector3(x_pos, y_pos, z_pos),
                        velocityMagnitude = float.Parse(values[4], culture),
                        velocityVector = new Vector3(vx, vy, vz)
                    });
                }
                catch (System.Exception ex) { Debug.LogWarning($"Could not parse line: '{line}'. Error: {ex.Message}"); }
            }
        }
        return dataPoints;
    }

    private string ReadDataFile(string dataFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        if (File.Exists(filePath))
        {
            try { return File.ReadAllText(filePath); }
            catch(System.Exception e) { Debug.LogError($"Read error: {e.Message}"); return null; }
        } else { Debug.LogError($"File not found: {filePath}"); return null; }
    }
}