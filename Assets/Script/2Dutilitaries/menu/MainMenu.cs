using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject canvasOptions;

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

    public void Quiz()
    {
        SceneLoader.SceneToLoad = "Quiz";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Options()
    {
        canvasOptions.SetActive(true);
    }
}