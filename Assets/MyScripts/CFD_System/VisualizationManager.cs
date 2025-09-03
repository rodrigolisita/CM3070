using UnityEngine;
using UnityEngine.UI;

public class VisualizationManager : MonoBehaviour
{
    [Header("Visualizer Scripts")]
    [Tooltip("Drag the GlyphVisualizer component here.")]
    public GlyphVisualizer glyphVisualizerScript;
    [Tooltip("Drag the ContourPlotVisualizer component here.")]
    public ContourPlotVisualizer contourPlotVisualizerScript;

    [Header("UI Toggles")]
    public Toggle vectorToggle;
    public Toggle contourToggle;

    [Header("Scale Multiplier")]
    public int scaleMultiplier;
    
    // This function runs once, right at the start of the scene
    void Start()
    {
        // Immediately sync the visualizers to match the initial state of the toggles
        SetVectorVisibility(vectorToggle.isOn);
        SetContourPlotVisibility(contourToggle.isOn);
        SetScale(scaleMultiplier);
    }

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

    // This function will be called by your 1x, 2x, and 5x toggles.
    public void SetScale(float scaleMultiplier)
    {
        // 'transform' here refers to the CFD_Visualizer's transform,
        // since this script is attached to it.
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }

}
