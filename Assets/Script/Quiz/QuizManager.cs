using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("UI")]
    [SerializeField] private TMP_Text compteurQuestionText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Button[] boutonsReponses;
    [SerializeField] private TMP_Text[] textesReponses;

    [Header("Message")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Résultat")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text scoreText;

    private int questionActuelle = 0;
    private int score = 0;

    private void Start()
    {
        messagePanel.SetActive(false);
        resultPanel.SetActive(false);

        AfficherQuestion();
    }

    private void AfficherQuestion()
    {
        QuizQuestion question = questions[questionActuelle];

        // Compteur
        compteurQuestionText.text =
            $"Question {questionActuelle + 1}/{questions.Length}";

        // Question
        questionText.text = question.question;

        // Réponses
        for (int i = 0; i < boutonsReponses.Length; i++)
        {
            boutonsReponses[i].interactable = true;

            textesReponses[i].text = question.reponses[i].texte;

            int index = i;

            boutonsReponses[i].onClick.RemoveAllListeners();
            boutonsReponses[i].onClick.AddListener(() => SelectionnerReponse(index));
        }
    }

    private void SelectionnerReponse(int index)
    {
        QuizAnswer reponse =
            questions[questionActuelle].reponses[index];

        // Empêche de répondre plusieurs fois
        DesactiverBoutons();

        // Ajouter les points si bonne réponse
        if (reponse.estCorrecte)
        {
            score++;
        }

        // Afficher le message
        messageText.text = reponse.message;
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
        resultPanel.SetActive(true);

        scoreText.text =
            $"Score : {score}/{questions.Length}";
    }
}