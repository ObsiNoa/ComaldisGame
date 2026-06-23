using UnityEngine;
using UnityEngine.UI;

public class RetourButtonController : MonoBehaviour
{
    public Button boutonRetour;

    void Start()
    {
        if (CanvasManager.Instance == null)
            Debug.LogError("CanvasManager.Instance is NULL");
        UpdateEtat();
    }

    void Update()
    {
        UpdateEtat();
    }

    void UpdateEtat()
    {
        if (CanvasManager.Instance == null || boutonRetour == null)
            return;

        bool planningBlocked =
            ClosePlanning.Instance != null &&
            ClosePlanning.Instance.IsPlanningOpen;

        bool canReturn =
            CanvasManager.Instance.PeutRetourner();

        bool dialogueStarted =
            DialogueManager.Instance != null &&
            DialogueManager.Instance.dialogueActuel != null &&
            DialogueManager.Instance.GetCurrentIndex() > 0;

        boutonRetour.interactable =
            !planningBlocked && (canReturn || dialogueStarted);
    }
}