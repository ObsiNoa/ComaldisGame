using UnityEngine;

[System.Serializable]
public class QuizAnswer
{
    public string texte;
    public bool estCorrecte;
    [TextArea]
    public string message;
}

[System.Serializable]
public class QuizQuestion
{
    [TextArea]
    public string question;

    public QuizAnswer[] reponses;
}