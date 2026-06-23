using UnityEngine;
using TMPro;

public class GestionnaireCheckList : MonoBehaviour
{
    public TextMeshProUGUI textePneus;
    public TextMeshProUGUI texteBache;

    public void CocherTache(string typeDeTache)
    {
        if(typeDeTache == "pneus" && textePneus != null)
        {
            textePneus.text = "[X] Pression des pneus";
            textePneus.color = Color.green;

        }
        else if (typeDeTache == "bache" && texteBache != null)
        {
            texteBache.text = "[X] Etat de la bâche";
            texteBache.color = Color.green; 
        }

        VerifierFinDeFormation(); 
    }

    void VerifierFinDeFormation()
    {
        // Optionnel : Si toutes les tâches sont au vert, on peut afficher un message "Prêt à partir !"
    }
}
