using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Image backgroundImage;
    public DialogueData dialogueActuel;
    public TMP_Text dialogueText;

    [Header("Dialogue Canvas (auto-hidden when dialogue ends without choices)")]
    public GameObject dialogueCanvas;

    private DialogueData previousDialogue;
    private int previousIndex;

    [Header("Choices UI (Pre-placed Buttons)")]
    private GameObject currentChoicePanel;
    private int index;
    private bool dialogueHasProgressed;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        previousDialogue = dialogueActuel;
        previousIndex = index;

        dialogueActuel = dialogue;
        index = 0;

        if (currentChoicePanel != null)
        {
            Destroy(currentChoicePanel);
            currentChoicePanel = null;
        }

        // Make sure the canvas is visible again when a new dialogue starts
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Dialogue;

        ShowLine();
    }

    void ShowLine()
    {
        if (dialogueActuel == null ||
            dialogueActuel.lignes == null ||
            dialogueActuel.lignes.Length == 0)
        {
            Debug.LogWarning("Dialogue vide.");
            StopDialogue();
            return;
        }

        dialogueText.text = dialogueActuel.lignes[index];
    }

    public void PreviousLine()
    {
        if (dialogueActuel == null) return;

        if (index > 0)
        {
            index--;
            ShowLine();
        }
        else if (previousDialogue != null)
        {
            dialogueActuel = previousDialogue;
            index = previousIndex;
            previousDialogue = null;
            ShowLine();
        }
    }

    public int GetCurrentIndex()
    {
        return index;
    }

    public void NextLine()
    {
        index++;

        if (CanvasManager.Instance != null)
            CanvasManager.Instance.NotifyDialogueProgress();

        if (index >= dialogueActuel.lignes.Length)
        {
            ShowChoicesOrEnd();
            return;
        }

        ShowLine();
    }

    void ChangeBackground(Sprite newBackground)
    {
        if (newBackground != null)
        {
            backgroundImage.sprite = newBackground;
        }
    }

    void ShowChoicesOrEnd()
    {
        if (dialogueActuel.nextBackground != null)
            ChangeBackground(dialogueActuel.nextBackground);

        if (dialogueActuel.choices != null && dialogueActuel.choices.Length > 0)
        {
            ShowChoices();
        }
        else
        {
            StopDialogue();
        }
    }

    void ShowChoices()
    {
        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Choix;

        GameObject panelPrefab = dialogueActuel.customChoicePanelPrefab;
        if (panelPrefab == null)
        {
            Debug.LogError($"Aucun Choice Panel assigné dans {dialogueActuel.name}");
            return;
        }

        if (currentChoicePanel != null)
            Destroy(currentChoicePanel);

        // Single instantiation at root, no parent
        currentChoicePanel = Instantiate(panelPrefab);

        Canvas panelCanvas = currentChoicePanel.GetComponent<Canvas>();
        if (panelCanvas == null)
            panelCanvas = currentChoicePanel.AddComponent<Canvas>();
        panelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        panelCanvas.sortingOrder = 999;

        if (currentChoicePanel.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            currentChoicePanel.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        Button[] buttons = currentChoicePanel.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < dialogueActuel.choices.Length; i++)
        {
            if (i >= buttons.Length)
            {
                Debug.LogWarning("Pas assez de boutons dans le panel !");
                break;
            }

            var choice = dialogueActuel.choices[i];
            Button btn = buttons[i];

            btn.gameObject.SetActive(true);

            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = choice.texte;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                Destroy(currentChoicePanel);
                currentChoicePanel = null;

                ChangeBackground(choice.backgroundAfterChoice);

                if (choice.nextDialogue != null)
                    StartDialogue(choice.nextDialogue);
                else
                    StopDialogue();
            });
        }
    }

    public void StopDialogue()
    {
        if (currentChoicePanel != null)
        {
            Destroy(currentChoicePanel);
            currentChoicePanel = null;
        }

        // Destroy the dialogue canvas instead of just disabling it
        if (dialogueCanvas != null)
        {
            Destroy(dialogueCanvas);
            dialogueCanvas = null;
        }

        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Zone;
    }
}