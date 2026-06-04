using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour
{
    public DialogueData dialogueInitial;

    IEnumerator Start()
    {
        Debug.Log("GameStart : Attente...");

        // On attend un frame pour laisser le temps aux Awake() de se lancer
        yield return null;

        if (DialogueManager.Instance == null)
            Debug.LogError("Erreur : DialogueManager.Instance est toujours null !");

        if (InputVN.Instance == null)
            Debug.LogError("Erreur : InputVN.Instance est toujours null !");

        while (DialogueManager.Instance == null || InputVN.Instance == null)
        {
            yield return null;
        }

        Debug.Log("Lancement OK");
        DialogueManager.Instance.StartDialogue(dialogueInitial);
    }
}