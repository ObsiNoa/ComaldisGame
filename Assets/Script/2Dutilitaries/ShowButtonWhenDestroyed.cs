using UnityEngine;
public class ActivateButtonOnDestroy : MonoBehaviour
{
    [Tooltip("Le bouton à activer quand ce panel est détruit")]
    public GameObject boutonAActiver;

    void OnDestroy()
    {
        if (boutonAActiver != null)
        {
            boutonAActiver.SetActive(true);
        }
    }
}

