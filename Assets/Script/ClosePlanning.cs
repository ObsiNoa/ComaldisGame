using UnityEngine;
using UnityEngine.UI;

public class ClosePlanning : MonoBehaviour
{
    public GameObject Planning;
    public Sprite CloseImage;
    public Sprite OpenImage;
    public Button PlanningButton;

    private bool PlanningOpen = true;

    private DialogueData savedDialogue; //currently open dialogue
    private bool wasInDialogue = false; 

    public void OpenAndClose()
    {
        PlanningOpen = !PlanningOpen;

        if (!PlanningOpen)
        {

            Planning.SetActive(false);
            PlanningButton.image.sprite = OpenImage;

            // Check if a dialogue is currently running
            if (DialogueManager.Instance != null &&
                DialogueManager.Instance.dialogueActuel != null)
            {
                wasInDialogue = true;

                savedDialogue = DialogueManager.Instance.dialogueActuel;
            }
            else
            {
                wasInDialogue = false;
            }
        }
        else
        {
            // Opening UI
            Planning.SetActive(true);
            PlanningButton.image.sprite = CloseImage;

            // Resume dialogue if needed
            if (wasInDialogue && savedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(savedDialogue);

                // restore line AFTER StartDialogue resets index
                RestoreIndexDelayed();
            }
        }
    }

    async void RestoreIndexDelayed()
    {
        await System.Threading.Tasks.Task.Yield();
        DialogueManager.Instance.NextLine(); // optional adjust below
    }
}