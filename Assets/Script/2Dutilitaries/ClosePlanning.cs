using UnityEngine;
using UnityEngine.UI;

public class ClosePlanning : MonoBehaviour
{
    public GameObject Planning;
    public Sprite CloseImage;
    public Sprite OpenImage;
    public Button PlanningButton;
    public Button boutonRetour;

    public bool IsPlanningOpen => planningOpen;
    public static ClosePlanning Instance;

    [Header("Dialogue lancé la première fois")]
    public DialogueData firstDialogue;

    private bool planningOpen = true;
    private bool firstDialogueStarted = false;

    private VNMode previousMode;

    public void Start()
    {
        boutonRetour.gameObject.SetActive(false);
    }

    void Awake()
    {
        Instance = this;
    }

    public void OpenAndClose()
    {
        planningOpen = !planningOpen;

        if (!planningOpen)
        {
            // Fermeture du planning
            Planning.SetActive(false);
            boutonRetour.gameObject.SetActive(true);
            boutonRetour.interactable = CanvasManager.Instance.PeutRetourner();
            PlanningButton.image.sprite = OpenImage;

            if (!firstDialogueStarted)
            {
                firstDialogueStarted = true;

                DialogueManager.Instance.StartDialogue(firstDialogue);
            }
            else
            {
                // Reprendre l'état précédent
                InputVN.Instance.modeActuel = previousMode;
            }
        }
        else
        {
            // Sauvegarder le mode actuel avant de geler le jeu
            previousMode = InputVN.Instance.modeActuel;

            // Ouverture du planning
            Planning.SetActive(true);
            boutonRetour.gameObject.SetActive(false);
            PlanningButton.image.sprite = CloseImage;

            // Gèle les interactions VN
            InputVN.Instance.modeActuel = VNMode.Zone;
        }
    }
}