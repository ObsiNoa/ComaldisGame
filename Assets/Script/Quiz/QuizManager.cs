using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("Panel Question")]
    [SerializeField] private GameObject questionPanel; // le panel vert avec compteur/question/réponses
    [SerializeField] private TMP_Text compteurQuestionText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] boutonsReponses;
    [SerializeField] private TMP_Text[] textesReponses; // "Réponse A :", "Réponse B :", "Réponse C :"

    private readonly string[] lettres = { "A", "B", "C" };

    [Header("Message / Feedback")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text verdictText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button boutonSuivant;
    [SerializeField] private TMP_Text boutonSuivantText;

    [Header("Résultat")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button boutonRejouer;
    [SerializeField] private Button boutonMenu;
    [SerializeField] private string nomSceneMenu = "MainMenu"; // nom exact de votre scène menu

    [Header("Références Layout")]
    [SerializeField] private RectTransform zoneReponsesRect;
    [SerializeField] private RectTransform questionPanelRect;

    [Header("Ajustement dynamique")]
    [SerializeField] private RectTransform panelRect;      // le panel vert entier
    [SerializeField] private RectTransform headerRect;      // titre + compteur
    [SerializeField] private RectTransform questionRect;    // le texte question
    [SerializeField] private float questionMaxFontSize = 40f;
    [SerializeField] private float questionMinFontSize = 28f;
    [SerializeField] private float paddingMargin = 20f; // marge de sécurité


    private int questionActuelle = 0;
    private int score = 0;

    private void Start()
    {
        messagePanel.SetActive(false);
        resultPanel.SetActive(false);

        boutonSuivant.onClick.AddListener(QuestionSuivante);
        boutonRejouer.onClick.AddListener(RejouerQuiz);
        boutonMenu.onClick.AddListener(RetourMenu);

        AfficherQuestion();
    }

    private void AjusterTaillesPourTenirDansLePanel()
    {
        // Forcer le calcul des tailles préférées de chaque bloc
        LayoutRebuilder.ForceRebuildLayoutImmediate(headerRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(zoneReponsesRect);

        float hauteurPanel = panelRect.rect.height;
        float hauteurHeader = headerRect.rect.height;
        float hauteurReponses = LayoutUtility.GetPreferredHeight(zoneReponsesRect);

        // Espace restant disponible pour la question
        float espaceDispoQuestion = hauteurPanel - hauteurHeader - hauteurReponses - paddingMargin;
        espaceDispoQuestion = Mathf.Max(espaceDispoQuestion, 20f); // jamais négatif

        TMP_Text questionText = questionRect.GetComponent<TMP_Text>();

        // On force la boîte de la question à cette hauteur précise
        questionRect.sizeDelta = new Vector2(questionRect.sizeDelta.x, espaceDispoQuestion);

        // Auto Size doit être activé sur ce TMP_Text avec Min/Max définis dans l'Inspector
        questionText.fontSizeMin = questionMinFontSize;
        questionText.fontSizeMax = questionMaxFontSize;
        questionText.enableAutoSizing = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(questionRect);
    }

    private void AfficherQuestion()
    {
        questionPanel.SetActive(true);
        messagePanel.SetActive(false);

        QuizQuestion question = questions[questionActuelle];

        compteurQuestionText.text = $"Question {questionActuelle + 1} / {questions.Length}";
        questionText.text = question.question;

        for (int i = 0; i < boutonsReponses.Length; i++)
        {
            boutonsReponses[i].interactable = true;

            if (i < question.reponses.Length && i < textesReponses.Length)
            {
                textesReponses[i].text = $"Réponse {lettres[i]} : {question.reponses[i].texte}";
            }

            int index = i;
            boutonsReponses[i].onClick.RemoveAllListeners();
            boutonsReponses[i].onClick.AddListener(() => SelectionnerReponse(index));
        }

        boutonSuivantText.text = (questionActuelle == questions.Length - 1)
            ? "Voir résultat"
            : "Question suivante";
        LayoutRebuilder.ForceRebuildLayoutImmediate(zoneReponsesRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(questionPanelRect);

        AjusterTaillesPourTenirDansLePanel();
    }

    private void SelectionnerReponse(int index)
    {
        QuizAnswer reponse = questions[questionActuelle].reponses[index];

        DesactiverBoutons();

        if (reponse.estCorrecte)
        {
            score++;
            verdictText.text = "Bonne réponse !";
        }
        else
        {
            verdictText.text = "Mauvaise réponse";
        }

        messageText.text = reponse.message;

        // Le panel réponse cache le panel question
        questionPanel.SetActive(false);
        messagePanel.SetActive(true);
    }

    private void DesactiverBoutons()
    {
        foreach (Button bouton in boutonsReponses)
        {
            bouton.interactable = false;
        }
    }

    public void QuestionSuivante()
    {
        questionActuelle++;
        messagePanel.SetActive(false);

        if (questionActuelle < questions.Length)
        {
            AfficherQuestion();
        }
        else
        {
            AfficherResultat();
        }
    }

    private void AfficherResultat()
    {
        questionPanel.SetActive(false);
        messagePanel.SetActive(false);
        for (int i = 0; i < boutonsReponses.Length; i++)
        {
            boutonsReponses[i].gameObject.SetActive(false);
        }

        resultPanel.SetActive(true);
        scoreText.text = $"Score : {score}/{questions.Length}";
    }

    private void RejouerQuiz()
    {
        questionActuelle = 0;
        score = 0;
        resultPanel.SetActive(false);
        for (int i = 0; i < boutonsReponses.Length; i++)
        {
            boutonsReponses[i].gameObject.SetActive(true);
        }
        AfficherQuestion();
    }

    private void RetourMenu()
    {
        SceneManager.LoadScene(nomSceneMenu);
    }
}