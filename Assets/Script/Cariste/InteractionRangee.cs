using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionRangee : MonoBehaviour
{
    [Header("UI Message Interaction")]
    public TextMeshProUGUI texteUI;
    public string messageInteraction = "Prendre un carton";
    public string messagePlein = "Vous portez déjà un carton !";

    [Header("UI Étiquette à afficher")]
    public GameObject etiquettePrefab;

    [Header("Gestion de la Rangée")]
    public List<GameObject> cartons = new List<GameObject>();

    private bool joueurDansLaZone = false;
    private ForkliftInventory inventaireForklift;

    private void Start()
    {
        // Remplissage automatique des enfants si la liste est vide
        if (cartons.Count == 0)
        {
            foreach (Transform child in transform)
            {
                cartons.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        // SÉCURITÉ : Si l'étiquette est ouverte, on ne fait rien
        if (FermerEtiquette.estOuverte)
        {
            return;
        }

        if (joueurDansLaZone && cartons.Count > 0)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Vérifier si le Forklift porte déjà un carton
                if (inventaireForklift != null && inventaireForklift.AUnCarton)
                {
                    Debug.Log("Impossible : Fourches déjà chargées !");
                    // On met à jour l'UI pour informer le joueur
                    if (texteUI != null) texteUI.text = messagePlein;
                }
                else
                {
                    PrendreCarton();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Forklift")
        {
            joueurDansLaZone = true;

            // Récupère le composant ForkliftInventory présent sur le joueur/forklift
            inventaireForklift = other.GetComponent<ForkliftInventory>();
            if (inventaireForklift == null)
            {
                inventaireForklift = other.GetComponentInParent<ForkliftInventory>();
            }

            MettreAJourTexteUI();
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

    void MettreAJourTexteUI()
    {
        if (texteUI == null) return;

        if (cartons.Count == 0)
        {
            texteUI.gameObject.SetActive(false);
            return;
        }

        texteUI.gameObject.SetActive(true);

        if (inventaireForklift != null && inventaireForklift.AUnCarton)
        {
            texteUI.text = messagePlein;
        }
        else
        {
            texteUI.text = "[E] " + messageInteraction;
        }
    }

    void PrendreCarton()
    {
        // 1. Récupérer le premier carton (index 0)
        GameObject cartonAPrendre = cartons[0];

        // 2. Le retirer de la liste
        cartons.RemoveAt(0);

        // 3. Attacher le carton au Forklift au lieu de le détruire
        if (inventaireForklift != null)
        {
            inventaireForklift.AttacherCarton(cartonAPrendre);
        }

        // 4. Afficher l'étiquette
        AfficherEtiquette();

        // 5. Mettre à jour le texte UI
        MettreAJourTexteUI();
    }

    void AfficherEtiquette()
    {
        if (etiquettePrefab != null)
        {
            GameObject nouvelleEtiquette = Instantiate(etiquettePrefab);
            nouvelleEtiquette.SetActive(true);
        }
    }
}