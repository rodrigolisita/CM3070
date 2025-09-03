using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;

/// <summary>
/// Acts as the central manager for CFD data. It handles UI interaction,
/// orchestrates file loading, and provides the master dataset to the rest of the application.
/// It uses a separate reader class (like TextFileReader) to do the actual parsing.
/// </summary>
public class CFD_DataProvider : MonoBehaviour
{
    [Header("Data Source")]
    public TMP_Dropdown dropdown;
    public List<string> simulationFilenames;

    // Public properties that other scripts can access for visualization.
    public List<DataPoint> DataPoints { get; private set; } = new List<DataPoint>();
    public float MinVelocity { get; private set; } = 0f;
    public float MaxVelocity { get; private set; } = 1f;
    public float MinZ { get; private set; } = 0f;
    public float MaxZ { get; private set; } = 1f;
    public bool IsDataReady { get; private set; } = false;

    // Event that other scripts (like the DataSlicer) can subscribe to.
    public event System.Action OnDataLoaded;

    void Awake()
    {
        // Automatically populate the dropdown menu when the scene starts.
        PopulateDropdown();
    }

    void Start()
    {
        // Load the first simulation file by default.
        if (simulationFilenames != null && simulationFilenames.Count > 0)
        {
            LoadDataFileByIndex(0);
        }
    }

    /// <summary>
    /// Populates the UI dropdown with user-friendly names from the filenames list.
    /// </summary>
    void PopulateDropdown()
    {
        if (dropdown == null) return;

        dropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (string filename in simulationFilenames)
        {
            string friendlyName = Path.GetFileNameWithoutExtension(filename).Replace("_", " ");
            options.Add(new TMP_Dropdown.OptionData(friendlyName));
        }
        dropdown.AddOptions(options);
    }

    /// <summary>
    /// Public method to be called by the UI Dropdown's OnValueChanged event.
    /// </summary>
    public void OnSimulationSelected()
    {
        if (dropdown == null) return;
        LoadDataFileByIndex(dropdown.value);
    }

    /// <summary>
    /// Orchestrates the data loading process.
    /// </summary>
    private async void LoadDataFileByIndex(int index)
    {
        if (index < 0 || index >= simulationFilenames.Count) return;

        IsDataReady = false;
        string selectedFile = simulationFilenames[index];
        string fileContent = await ReadDataFileAsync(selectedFile);

        if (string.IsNullOrEmpty(fileContent)) return;

        // Use our specialized, static TextFileReader to parse the content.
        DataPoints = TextFileReader.ReadAndParseFile(fileContent);

        if (DataPoints.Count == 0)
        {
            Debug.LogError("No data points were parsed from the file.");
            return;
        }

        // Calculate global properties from the new dataset.
        MinVelocity = DataPoints.Min(p => p.velocityMagnitude);
        MaxVelocity = DataPoints.Max(p => p.velocityMagnitude);
        MinZ = DataPoints.Min(p => p.position.z);
        MaxZ = DataPoints.Max(p => p.position.z);

        // Notify all subscribed listeners that the new data is ready.
        IsDataReady = true;
        OnDataLoaded?.Invoke();
    }

    /// <summary>
    /// Asynchronously reads a file from the StreamingAssets folder in a platform-agnostic way.
    /// </summary>
    private async Task<string> ReadDataFileAsync(string dataFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);

        // On platforms like Android and WebGL, we must use UnityWebRequest.
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            using (var www = UnityWebRequest.Get(filePath))
            {
                var asyncOp = www.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load file at {filePath}: {www.error}");
                    return null;
                }
                return www.downloadHandler.text;
            }
        }
        // For Editor and PC builds, we can use standard file IO.
        else
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Read error: {e.Message}");
                return null;
            }
        }
    }
}
