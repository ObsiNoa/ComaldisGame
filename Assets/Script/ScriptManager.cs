using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueData dialogueActuel;
    public TMP_Text dialogueText;

    [Header("Choices UI")]
    public GameObject choicePanel;
    public Button choiceButtonPrefab;
    public Transform choiceContainer;

    private int index;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        dialogueActuel = dialogue;
        index = 0;

        choicePanel.SetActive(false);
        InputVN.Instance.modeActuel = VNMode.Dialogue;

        ShowLine();
    }

    void ShowLine()
    {
        dialogueText.text = dialogueActuel.lignes[index];
    }

    public void NextLine()
    {
        index++;

        if (index >= dialogueActuel.lignes.Length)
        {
            ShowChoicesOrEnd();
            return;
        }

        ShowLine();
    }

    void ShowChoicesOrEnd()
    {
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

        choicePanel.SetActive(true);

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var choice in dialogueActuel.choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TMP_Text>().text = choice.texte;

            btn.onClick.AddListener(() =>
            {
                StartDialogue(choice.nextDialogue);
            });
        }
    }

    public void StopDialogue()
    {
        choicePanel.SetActive(false);
        InputVN.Instance.modeActuel = VNMode.Zone;
    }
}