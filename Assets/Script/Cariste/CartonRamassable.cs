using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CartonRamassable : MonoBehaviour
{
    [Header("UI Message")]
    public string messageRamasser = "Ramasser le carton";

    [Header("UI Étiquette")]
    public GameObject etiquettePrefab;

    [Header("Distance de ramassage")]
    [Tooltip("Distance en mètres pour pouvoir re-ramasser ce carton au sol")]
    public float distanceRamassage = 3.5f;

    private ForkliftInventory inventaireForklift;
    private TextMeshProUGUI texteUI;
    private bool joueurProche = false;

    private void OnEnable()
    {
        // Réinitialiser proprement l'état dès que le carton est posé par terre
        joueurProche = false;
    }

    private void OnDisable()
    {
        // Masquer l'UI si le script est désactivé
        if (texteUI != null && joueurProche)
        {
            texteUI.gameObject.SetActive(false);
        }
        joueurProche = false;
    }

    void Update()
    {
        if (FermerEtiquette.estOuverte) return;

        // Récupération automatique des références au besoin
        if (inventaireForklift == null)
        {
            inventaireForklift = FindFirstObjectByType<ForkliftInventory>();
            if (inventaireForklift == null) return;
        }

        if (texteUI == null)
        {
            texteUI = FindFirstObjectByType<TextMeshProUGUI>();
        }

        // Calcul de la distance entre le carton au sol et le transpalette
        float distance = Vector3.Distance(transform.position, inventaireForklift.transform.position);

        // Si on est à côté du carton ET que le transpalette n'a pas déjà un carton
        if (distance <= distanceRamassage && !inventaireForklift.AUnCarton)
        {
            if (!joueurProche)
            {
                joueurProche = true;
                AfficherUI(true);
            }

            // Touche [E] pour ramasser au sol
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                RamasserCeCarton();
            }
        }
        else
        {
            if (joueurProche)
            {
                joueurProche = false;
                AfficherUI(false);
            }
        }
    }

    void AfficherUI(bool afficher)
    {
        if (texteUI == null) return;

        if (afficher)
        {
            texteUI.text = "[E] " + messageRamasser;
            texteUI.gameObject.SetActive(true);
        }
        else
        {
            texteUI.gameObject.SetActive(false);
        }
    }

    void RamasserCeCarton()
    {
        AfficherUI(false);

        if (inventaireForklift != null)
        {
            inventaireForklift.AttacherCarton(gameObject);
        }

        if (etiquettePrefab != null)
        {
            GameObject nouvelleEtiquette = Instantiate(etiquettePrefab);
            nouvelleEtiquette.SetActive(true);
        }

        // Désactiver la détection au sol pendant qu'on le porte
        this.enabled = false;
    }
}