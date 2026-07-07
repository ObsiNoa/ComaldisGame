using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Cette variable va garder en mémoire le panel actuellement ouvert à l'écran
    private GameObject panelActuelOuvert;

    public void OuvrirUnPanel(GameObject panelAActiver)
    {
        // 1. Si un panel est déjà ouvert à l'écran, on le ferme d'abord
        if (panelActuelOuvert != null)
        {
            panelActuelOuvert.SetActive(false);
        }

        // 2. On ouvre le nouveau panel demandé
        if (panelAActiver != null)
        {
            panelAActiver.SetActive(true);

            // 3. On se souvient que c'est ce panel qui est maintenant ouvert
            panelActuelOuvert = panelAActiver;
        }
    }

    public void FermerUnPanel(GameObject panelAFermer)
    {
        if (panelAFermer != null)
        {
            panelAFermer.SetActive(false);

            // Si le panel qu'on ferme était celui en mémoire, on vide la mémoire
            if (panelActuelOuvert == panelAFermer)
            {
                panelActuelOuvert = null;
            }
        }
    }
}