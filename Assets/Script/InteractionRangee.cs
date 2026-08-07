using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionRangee : MonoBehaviour
{
    [Header("UI Message Interaction")]
    public TextMeshProUGUI texteUI;
    public string messageInteraction = "Prendre un carton";

    [Header("UI Étiquette à afficher")]
    public GameObject etiquettePrefab;
    public Transform uiCanvasParent; // Le Canvas ou le conteneur où ajouter l'étiquette

    [Header("Gestion de la Rangée")]
    [Tooltip("Dépose ici les cartons de cette rangée, du plus proche au plus éloigné.")]
    public List<GameObject> cartons = new List<GameObject>();

    private bool joueurDansLaZone = false;

    private void Start()
    {
        // Si la liste est vide dans l'inspecteur, on prend automatiquement tous les enfants
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
        // SÉCURITÉ : Si une étiquette est affichée à l'écran, on bloque l'interaction !
        if (FermerEtiquette.estOuverte)
        {
            return;
        }

        // Le reste du code d'interaction habituel :
        if (joueurDansLaZone && cartons.Count > 0)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                PrendreCarton();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            joueurDansLaZone = true;

            // N'afficher l'invite que s'il reste des cartons
            if (cartons.Count > 0 && texteUI != null)
            {
                texteUI.text = "[E] " + messageInteraction;
                texteUI.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            joueurDansLaZone = false;

            if (texteUI != null)
            {
                texteUI.gameObject.SetActive(false);
            }
        }
    }

    void PrendreCarton()
    {
        // 1. On récupère le PREMIER carton de la liste (index 0)
        GameObject cartonAPrendre = cartons[0];

        // 2. On le retire de la liste (le 2ème devient automatiquement le 1er pour le prochain appui)
        cartons.RemoveAt(0);

        // 3. On détruit ou désactive le carton dans la scène 3D
        Destroy(cartonAPrendre);

        // 4. On affiche le préfab de l'étiquette à l'écran
        AfficherEtiquette();

        // 5. S'il n'y a plus du tout de cartons dans la rangée, on cache le message
        if (cartons.Count == 0 && texteUI != null)
        {
            texteUI.gameObject.SetActive(false);
        }
    }

    void AfficherEtiquette()
    {
        if (etiquettePrefab != null)
        {
            // On instancie la préfab SANS lui donner de parent (elle devient son propre Canvas autonome)
            GameObject nouvelleEtiquette = Instantiate(etiquettePrefab);

            // On s'assure qu'elle est bien active
            nouvelleEtiquette.SetActive(true);
        }
    }
}