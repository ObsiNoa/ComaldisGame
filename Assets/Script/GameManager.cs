using UnityEngine;
using System.Collections;
public class GameStart : MonoBehaviour
{
    public DialogueData dialogueInitial;

    IEnumerator Start()
    {
        while (DialogueManager.Instance == null)
            yield return null;

        if (dialogueInitial == null)
            {
                Debug.LogError("dialogueInitial is null! Assign a DialogueData asset in the Inspector.");
                yield break;
            }
        DialogueManager.Instance.StartDialogue(dialogueInitial);
    }
 }