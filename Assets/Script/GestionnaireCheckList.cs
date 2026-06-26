using UnityEngine;
using TMPro;

public class GestionnaireCheckList : MonoBehaviour
{
    [Header("Textes CheckList")]
    public TextMeshProUGUI textePneus;
    public TextMeshProUGUI texteBache;
    public TextMeshProUGUI textePorte;
    public TextMeshProUGUI textePhare;

    [Header("UI Finale")]
    public GameObject texteVictoire;

    [Header("Effect finale")]
    public GameObject particulesPorteCamion;

    private int tachesAccomplies = 0;
    private const int TOTAL_TACHES = 4; 

    public void CocherTache(string typeDeTache)
    {
        if(typeDeTache == "pneus" && textePneus != null)
        {
            textePneus.text = "[X] Pression des pneus";
            textePneus.color = Color.green;
            tachesAccomplies++; 

        }
        else if (typeDeTache == "bache" && texteBache != null)
        {
            texteBache.text = "[X] Etat de la bâche";
            texteBache.color = Color.green;
            tachesAccomplies++;
        }
        else if (typeDeTache == "porte" && textePorte != null)
        {
            textePorte.text = "[X] Fermeture de la porte";
            textePorte.color = Color.green;
            tachesAccomplies++;
        }
        else if(typeDeTache == "phare" && textePhare != null)
        {
            textePhare.text = "[X] Etat des phares";
            textePhare.color = Color.green;
            tachesAccomplies++;
        }

            VerifierFinDeFormation(); 
    }

    void VerifierFinDeFormation()
    {
        if(tachesAccomplies >= TOTAL_TACHES)
        {
            if(texteVictoire != null)
            {
                texteVictoire.SetActive(true);
            }

            if(particulesPorteCamion != null)
            {
                particulesPorteCamion.SetActive(true);
            }
        }
    }
}
