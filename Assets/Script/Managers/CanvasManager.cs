using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    private Stack<GameObject> historique = new Stack<GameObject>();
    public GameObject canvasActuel;

    private bool dialogueHasProgressed = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (canvasActuel != null)
            canvasActuel.SetActive(true);

        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Zone;
    }

    public bool PeutRetourner()
    {
        return dialogueHasProgressed || historique.Count > 0;
    }

    public void AllerVers(GameObject nouveauCanvas)
    {
        if (nouveauCanvas == null) return;

        if (canvasActuel != null && canvasActuel != nouveauCanvas)
        {
            historique.Push(canvasActuel);
            canvasActuel.SetActive(false);
        }

        canvasActuel = nouveauCanvas;
        canvasActuel.SetActive(true);
    }

    public void Retour()
    {
        // 1. UI back
        if (historique.Count > 0)
        {
            canvasActuel.SetActive(false);
            canvasActuel = historique.Pop();
            canvasActuel.SetActive(true);
        }

        // 2. dialogue back (safe check)
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.PreviousLine();
        }

        // 3. IMPORTANT: prevent "always true" state
        if (historique.Count == 0)
        {
            dialogueHasProgressed = false;
        }
    }

    public void NotifyDialogueProgress()
    {
        dialogueHasProgressed = true;
    }

    public void ResetDialogueProgress()
    {
        dialogueHasProgressed = false;
    }
}