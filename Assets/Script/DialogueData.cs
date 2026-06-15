using UnityEngine;
using System;
using UnityEngine.UIElements;


[CreateAssetMenu(menuName = "VN/Dialogue")]

public class DialogueData : ScriptableObject
{
    [TextArea]
    public string[] lignes;

    public DialogueChoice[] choices;

    public Sprite nextBackground;

    [Header("Prefab UI (Canvas Panel)")]
    public GameObject customChoicePanelPrefab;
}

[Serializable]
public class DialogueChoice
{
    public string texte;
    public DialogueData nextDialogue;

    [Header("Fond après ce choix")]
    public Sprite backgroundAfterChoice;
}