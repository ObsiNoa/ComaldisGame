using UnityEngine;
using UnityEngine.UI;

public class RetourButtonController : MonoBehaviour
{
    public Button boutonRetour;

    void Start()
    {
        UpdateEtat();
    }

    void Update()
    {
        UpdateEtat();
    }

    void UpdateEtat()
    {
        boutonRetour.interactable = CanvasManager.Instance.PeutRetourner();
    }
}