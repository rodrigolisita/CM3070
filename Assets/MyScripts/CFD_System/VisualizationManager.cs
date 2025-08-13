using UnityEngine;

public class VisualizationManager : MonoBehaviour
{
    [Header("Visualizer Scripts")]
    [Tooltip("Drag the GlyphVisualizer component here.")]
    public GlyphVisualizer glyphVisualizerScript;
    [Tooltip("Drag the ContourPlotVisualizer component here.")]
    public ContourPlotVisualizer contourPlotVisualizerScript;
    
    // This function will be called by the "Show Vectors" UI Toggle
    public void SetVectorVisibility(bool isOn)
    {
        Debug.Log("SetVectorVisibility called with value: " + isOn);

        if (glyphVisualizerScript != null)
        {
            glyphVisualizerScript.enabled = isOn;

            if (isOn)
            {
                glyphVisualizerScript.DrawGlyphs(); // force a draw
            }
            else
            {
                glyphVisualizerScript.ClearGlyphs(); // hide everything
            }
        }
        
        
    }

    // This function will be called by the "Show Contour Plot" UI Toggle
    public void SetContourPlotVisibility(bool isOn)
    {
    Debug.Log("SetContourPlotVisibility called with value: " + isOn);

    if (contourPlotVisualizerScript != null)
    {
        contourPlotVisualizerScript.enabled = isOn;

        if (contourPlotVisualizerScript.targetRenderer != null)
        {
            contourPlotVisualizerScript.targetRenderer.gameObject.SetActive(isOn);
        }

        if (isOn)
        {
            contourPlotVisualizerScript.GenerateContour();
        }
    }
    }

}
