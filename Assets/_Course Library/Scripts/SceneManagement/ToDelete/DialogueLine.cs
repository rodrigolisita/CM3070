using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)] // Makes the string field larger in the inspector
    public string textContent;
    public AudioClip narrationClip;
    // Add other relevant data per line, e.g., speaker portrait, text speed, etc.
}