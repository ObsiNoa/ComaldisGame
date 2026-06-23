using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Image backgroundImage;
    public DialogueData dialogueActuel;
    public TMP_Text dialogueText;

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
        dialogueActuel = dialogue;
        index = 0;

        if (currentChoicePanel != null)
        {
            Destroy(currentChoicePanel);
            currentChoicePanel = null;
        }
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

        if (index <= 0) return;

        index--;
        ShowLine();
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
        InputVN.Instance.modeActuel = VNMode.Choix;

        GameObject panelPrefab = dialogueActuel.customChoicePanelPrefab;

        if (panelPrefab == null)
        {
            Debug.LogError($"Aucun Choice Panel assigné dans {dialogueActuel.name}");
            return;
        }

        if (currentChoicePanel != null)
        {
            Destroy(currentChoicePanel);
        }

        Canvas parentCanvas = FindFirstObjectByType<Canvas>();
        Transform parent = parentCanvas != null ? parentCanvas.transform : transform;

        currentChoicePanel = Instantiate(panelPrefab, parent);

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

        InputVN.Instance.modeActuel = VNMode.Zone;
    }

}

