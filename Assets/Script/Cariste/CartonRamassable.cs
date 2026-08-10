using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CartonRamassable : MonoBehaviour
{
    [Header("UI Message")]
    public string messageRamasser = "Ramasser le carton";

    [Header("UI Étiquette")]
    public GameObject etiquettePrefab;

    private bool joueurDansLaZone = false;
    private ForkliftInventory inventaireForklift;
    private TextMeshProUGUI texteUI;

    void Update()
    {
        // Ne rien faire si une étiquette est déjà ouverte
        if (FermerEtiquette.estOuverte) return;

        if (joueurDansLaZone)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Vérifier si le transpalette est vide
                if (inventaireForklift != null && !inventaireForklift.AUnCarton)
                {
                    RamasserCeCarton();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Forklift")
        {
            joueurDansLaZone = true;

            // Trouver le ForkliftInventory
            inventaireForklift = other.GetComponent<ForkliftInventory>();
            if (inventaireForklift == null)
            {
                inventaireForklift = other.GetComponentInParent<ForkliftInventory>();
            }

            // Récupérer le texte UI depuis la scène s'il existe
            texteUI = FindFirstObjectByType<TextMeshProUGUI>();

            MettreAJourUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Forklift")
        {
            joueurDansLaZone = false;
            inventaireForklift = null;

            if (texteUI != null)
            {
                texteUI.gameObject.SetActive(false);
            }
        }
    }

    void MettreAJourUI()
    {
        if (texteUI == null) return;

        if (inventaireForklift != null && !inventaireForklift.AUnCarton)
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
        // 1. Charger le carton sur le transpalette
        inventaireForklift.AttacherCarton(gameObject);

        // 2. Afficher l'étiquette
        if (etiquettePrefab != null)
        {
            GameObject nouvelleEtiquette = Instantiate(etiquettePrefab);
            nouvelleEtiquette.SetActive(true);
        }

        // 3. Masquer le texte d'interaction
        if (texteUI != null) texteUI.gameObject.SetActive(false);

        // 4. Désactiver ce script pour ne pas qu'il s'active pendant le transport
        this.enabled = false;
    }

    private void OnEnable()
    {
        // S'assurer que le script s'active quand le carton est au sol
        joueurDansLaZone = false;
    }
}