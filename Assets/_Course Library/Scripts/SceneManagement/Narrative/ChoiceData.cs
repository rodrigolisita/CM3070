using UnityEngine;

[System.Serializable]
public class ChoiceData
{
    [Tooltip("The text displayed on the choice button.")]
    public string choiceText;

    [Tooltip("The array index of the StoryLine to jump to if this choice is selected.")]
    public int nextLineIndex;
}