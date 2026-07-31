using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    [Tooltip("Nom exact de la scène à charger (doit être dans Build Settings)")]
    public string nomDeLaScene;

    public void ChargerScene()
    {
        if (string.IsNullOrEmpty(nomDeLaScene))
        {
            Debug.LogWarning("Aucun nom de scène renseigné !");
            return;
        }

        SceneLoader.SceneToLoad = nomDeLaScene;
        SceneManager.LoadScene("LoadingScreen");
    }
}