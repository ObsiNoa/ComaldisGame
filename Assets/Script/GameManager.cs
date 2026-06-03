using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PanelTexte;

    public void OuvrirLePanel()
    {
        Debug.Log("Bouton Cliqué : la fonction OuvrirLePanel se lance");
        if(PanelTexte != null)
        {
            PanelTexte.SetActive(true); // rend visible le panel
            Debug.Log("le panel" + PanelTexte.name + "a été activé ");
        }
    }

}
