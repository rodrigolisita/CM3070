using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FluidController : MonoBehaviour
{
    public ForceVisualizer forceVisualizer;

    // Change the 'name' from string to LocalizedString
    [System.Serializable]
    public class Fluid
    {
        public LocalizedString name; //
        public float density; // in kg/m³
    }

    public List<Fluid> fluids = new List<Fluid>();
    public TMP_Dropdown fluidDropdown;
    public float CurrentFluidDensity { get; private set; }

    void Start()
    {
        // Add a listener to populate the dropdown once the localization system is ready
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        fluidDropdown.onValueChanged.AddListener(OnFluidSelectionChanged);
        
        PopulateDropdown();
    }

    private void OnLocaleChanged(Locale obj)
    {
        // Repopulate the dropdown whenever the user changes the language
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        fluidDropdown.ClearOptions();
        List<string> optionLabels = new List<string>();

        // Get the translated string for each fluid
        foreach (var fluid in fluids)
        {
            // GetLocalizedString() fetches the correct translation
            optionLabels.Add(fluid.name.GetLocalizedString()); 
        }

        fluidDropdown.AddOptions(optionLabels);
        
        // Ensure the current density is correct for the initial selection
        OnFluidSelectionChanged(fluidDropdown.value);
    }

    public void OnFluidSelectionChanged(int index)
    {
        if (index < 0 || index >= fluids.Count) return;

        CurrentFluidDensity = fluids[index].density;
        Debug.Log($"Fluid changed to: {fluids[index].name.GetLocalizedString()}, Density: {CurrentFluidDensity}");

        forceVisualizer.UpdateForceCalculations();
    }
}