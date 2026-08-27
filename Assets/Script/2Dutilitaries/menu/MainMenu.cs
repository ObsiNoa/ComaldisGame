using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject canvasOptions;
    [SerializeField] private GameObject PanelChoix;

    public void Jouer()
    {
        PanelChoix.SetActive(true);
        //SceneLoader.SceneToLoad = "MainHub";
        //SceneManager.LoadScene("LoadingScreen");
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

    public void LoadChauffeur()
    {
        SceneLoader.SceneToLoad = "MainHub";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void LoadCariste()
    {
        SceneLoader.SceneToLoad = "MainCariste";
        SceneManager.LoadScene("LoadingScreen");
    }
}
