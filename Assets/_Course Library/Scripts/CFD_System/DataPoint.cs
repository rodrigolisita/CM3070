using UnityEngine;

/// <summary>
/// This class acts as a standardized "data contract" for a single point of CFD data.
/// Data Readers are responsible for creating these objects, and the Data Provider manages them.
/// </summary>
public class DataPoint
{
    public Vector3 position;
    public Vector3 velocityVector;
    public float velocityMagnitude;
    public float pressure; // Added for future extensibility
}