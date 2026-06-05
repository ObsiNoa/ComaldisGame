using UnityEngine;
using System;

[CreateAssetMenu(menuName = "VN/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea]
    public string[] lignes;

    public DialogueChoice[] choices;

    [Header("Background à appliquer après ce dialogue")]
    public Sprite nextBackground;
}

[Serializable]
public class DialogueChoice
{
    public string texte;
    public DialogueData nextDialogue;

    [Header("Fond après ce choix")]
    public Sprite backgroundAfterChoice;
}