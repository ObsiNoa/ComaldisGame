using System;
using UnityEngine;

[CreateAssetMenu(menuName = "VN/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] lignes;

    public Choice[] choices;
}

[Serializable]
public class Choice
{
    public string texte;
    public DialogueData nextDialogue;
}