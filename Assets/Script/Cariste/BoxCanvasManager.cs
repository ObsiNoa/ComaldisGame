using UnityEngine;
using TMPro;

public class GlobalCanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text compteurText;

    private int compteur = 0;
    private const int nombreTotal = 12;

    private void Start()
    {
        ActualiserUI();
    }

    public void AjouterBoite()
    {
        compteur++;
        ActualiserUI();
    }

    public void RetirerBoite()
    {
        compteur = Mathf.Max(0, compteur - 1);
        ActualiserUI();
    }

    private void ActualiserUI()
    {
        compteurText.text = compteur + "/" + nombreTotal;
    }
}