using UnityEngine;
using System.Collections.Generic;

// This script requires a CFD_DataSource to be on the same GameObject.
[RequireComponent(typeof(CFD_DataSource))]
public class VectorPlotVisualizer_Gizmo : MonoBehaviour
{
    [Header("Visualization Settings")]
    [Tooltip("Color gradient for velocity magnitude.")]
    public Gradient velocityGradient;
    [Tooltip("Multiplier for the length of the vector lines.")]
    public float vectorLengthMultiplier = 0.5f; // Increased default
    [Tooltip("Normalize colors against a fixed global range.")]
    public bool useGlobalMinMax = false;
    [Tooltip("The global minimum velocity for the color gradient.")]
    public float globalMinVelocity = 0f;
    [Tooltip("The global maximum velocity for the color gradient.")]
    public float globalMaxVelocity = 2.0f;

    // --- Private Fields ---
    private CFD_DataSource dataSource;

    void Awake()
    {
        // Get a reference to the data source script on this GameObject
        dataSource = GetComponent<CFD_DataSource>();
    }

    // This special Unity function is called to draw gizmos in the editor.
    private void OnDrawGizmos()
    {
        if (dataSource == null || !dataSource.IsDataReady)
        {
            return;
        }

        List<CFD_DataSource.DataPoint> points = dataSource.DataPoints;
        float minV = useGlobalMinMax ? globalMinVelocity : dataSource.MinVelocity;
        float maxV = useGlobalMinMax ? globalMaxVelocity : dataSource.MaxVelocity;
        float velocityRange = maxV - minV;
        if (velocityRange < float.Epsilon) velocityRange = 1f;

        // Gizmos are drawn in world space. To make them relative to this object,
        // we apply the object's own transformation matrix.
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var point in points)
        {
            // Set the color for this specific Gizmo line
            Gizmos.color = velocityGradient.Evaluate((point.velocityMagnitude - minV) / velocityRange);

            // Draw one ray (a line with a direction) for each data point
            Gizmos.DrawRay(point.position, point.velocityVector * vectorLengthMultiplier);
        }
    }
}