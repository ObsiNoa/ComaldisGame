using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Jouer()
    {
        SceneLoader.SceneToLoad = "MainHub";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Quitter()
    {
        Debug.Log("Fermeture du jeu");

        Application.Quit();
    }
}