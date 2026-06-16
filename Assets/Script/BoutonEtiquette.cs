using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonEtiquette : MonoBehaviour
{
    [Header("Option 1 : Changer de scène")]
    public bool doitChangerDeScene = false;
    public string nomDeLaScene;

  
    public void ActionClic()
    {
        if (doitChangerDeScene)
        {
            // Mode Changement de scène
            SceneManager.LoadScene(nomDeLaScene);
        }
    }
}
