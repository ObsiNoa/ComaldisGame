using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueData dialogueActuel;
    public TMP_Text dialogueText;

    [Header("Choices UI (Pre-placed Buttons)")]
    public GameObject choicePanel;
    public Transform choiceContainer; // Ton panel qui contient directement tes boutons physiques

    private Button[] boutonsPrePlaces;
    private int index;

    void Awake()
    {
            Instance = this;
            boutonsPrePlaces = choiceContainer.GetComponentsInChildren<Button>(true);

            // Ligne de sécurité à ajouter :
            Debug.Log("Nombre de boutons trouvés dans le container : " + boutonsPrePlaces.Length);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        dialogueActuel = dialogue;
        index = 0;

        choicePanel.SetActive(false);
        if (InputVN.Instance != null)
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

        // On commence par désactiver TOUS les boutons pré-placés
        foreach (Button btn in boutonsPrePlaces)
        {
            btn.gameObject.SetActive(false);
        }

        // On configure uniquement les boutons nécessaires pour ce choix précis
        for (int i = 0; i < dialogueActuel.choices.Length; i++)
        {
            // Sécurité : si tu as plus de choix dans ton scriptable object que de boutons dans la scène
            if (i >= boutonsPrePlaces.Length)
            {
                Debug.LogWarning("Attention: Pas assez de boutons pré-placés pour afficher tous les choix !");
                break;
            }

            Button btn = boutonsPrePlaces[i];
            var choice = dialogueActuel.choices[i]; // Copie locale pour éviter le bug de capture de variable

            // On active le bouton et on change son texte
            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TMP_Text>().text = choice.texte;

            // On nettoie les anciens événements et on ajoute le nouveau
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                StartDialogue(choice.nextDialogue);
            });
        }
    }

    public void StopDialogue()
    {
        choicePanel.SetActive(false);
        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Zone;
    }
}