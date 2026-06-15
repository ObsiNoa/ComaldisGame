using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonEtiquette : MonoBehaviour
{
    [Header("Option 1 : Ouvrir un visuel")]
    public GameObject visuelEtiquette; 

    [Header("Option 2 : Changer de scène")]
    public bool doitChangerDeScene = false;
    public string nomDeLaScene;

  
    public void ActionClic()
    {
        if (doitChangerDeScene)
        {
            // Mode Changement de scène
            SceneManager.LoadScene(nomDeLaScene);
        }
        else if (visuelEtiquette != null)
        {
            // Mode Ouverture simple du visuel dans la même scène
            visuelEtiquette.SetActive(true);
        }
    }
}
