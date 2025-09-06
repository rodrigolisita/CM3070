using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

/// <summary>
/// A utility class responsible for reading and parsing CFD data from a specific
/// text file format (exported from Ansys). This class is decoupled from the Unity scene
/// and has a single responsibility: data parsing.
/// </summary>
public static class TextFileReader
{
    /// <summary>
    /// Reads the raw string content of a CFD data file and parses it into a list of DataPoint objects.
    /// </summary>
    /// <param name="fileContent">The full text content of the data file.</param>
    /// <returns>A list of structured DataPoint objects.</returns>
    public static List<DataPoint> ReadAndParseFile(string fileContent)
    {
        var dataPoints = new List<DataPoint>();

        // Use a StringReader for efficient line-by-line reading.
        using (var reader = new StringReader(fileContent))
        {
            // First line is a header, so we read and discard it.
            string line = reader.ReadLine();

            // Loop through the rest of the lines in the file.
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');

                // Ensure the line has enough columns to parse.
                if (values.Length < 8) continue;

                try
                {
                    // Use CultureInfo.InvariantCulture to ensure '.' is always the decimal separator,
                    // regardless of the user's system region.
                    var culture = CultureInfo.InvariantCulture;
                    float x_pos = float.Parse(values[1], culture);
                    float y_pos = float.Parse(values[2], culture);
                    float z_pos = float.Parse(values[3], culture);
                    float vx = float.Parse(values[5], culture);
                    float vy = float.Parse(values[6], culture);
                    float vz = float.Parse(values[7], culture);

                    // Create a new DataPoint and add it to our list.
                    // Note: Pressure is not in our current file, so we default it to 0.
                    dataPoints.Add(new DataPoint
                    {
                        position = new Vector3(x_pos, y_pos, z_pos),
                        velocityMagnitude = float.Parse(values[4], culture),
                        velocityVector = new Vector3(vx, vy, vz),
                        pressure = 0f 
                    });
                }
                catch (System.Exception ex)
                {
                    // If a line is malformed, log a warning but continue processing the rest of the file.
                    Debug.LogWarning($"Could not parse line: '{line}'. Error: {ex.Message}");
                }
            }
        }
        return dataPoints;
    }
}

