using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Jouer()
    {
        SceneManager.LoadScene("MainHub");
    }

    public void Quitter()
    {
        Debug.Log("Fermeture du jeu");

        Application.Quit();
    }
}