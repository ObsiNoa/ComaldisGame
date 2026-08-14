using System.Collections;
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

    [Header("UI Étiquettes")]
    public GameObject etiquetteClassiqueUI;
    public GameObject etiquetteDangereuseUI;

    [Header("Gestion de la Rangée")]
    public List<GameObject> cartons = new List<GameObject>();

    private bool joueurDansLaZone = false;
    private ForkliftInventory inventaireForklift;

    private bool interactionEnCours = false;

    private void Start()
    {
        // 1. Recherche automatique des UI dans la scène si non assignées
        if (etiquetteClassiqueUI == null)
        {
            etiquetteClassiqueUI = GameObject.Find("Etiquette");
        }

        if (etiquetteDangereuseUI == null)
        {
            etiquetteDangereuseUI = GameObject.Find("ADR");
        }

        // 2. Remplissage automatique des cartons de la rangée
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
        if (FermerEtiquette.estOuverte)
        {
            return;
        }

        if (joueurDansLaZone && cartons.Count > 0)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (inventaireForklift != null && inventaireForklift.AUnCarton)
                {
                    Debug.Log("Impossible : Fourches déjà chargées !");

                    if (texteUI != null)
                        texteUI.text = messagePlein;
                }
                else if (!interactionEnCours)
                {
                    StartCoroutine(PrendreCartonAvecDelai());
                }
            }
        }
    }

    private IEnumerator PrendreCartonAvecDelai()
    {
        interactionEnCours = true;

        // Attend 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        // Vérifie encore que le joueur peut prendre le carton
        if (joueurDansLaZone && cartons.Count > 0)
        {
            PrendreCarton();
        }

        interactionEnCours = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Forklift")
        {
            joueurDansLaZone = true;

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
        // 1. Récupérer le premier carton
        GameObject cartonAPrendre = cartons[0];

        // 2. Retirer de la liste
        cartons.RemoveAt(0);

        // 3. Attacher au Forklift
        if (inventaireForklift != null)
        {
            inventaireForklift.AttacherCarton(cartonAPrendre);
        }

        // 4. Activer l'étiquette déjà présente dans le Canvas
        OuvrirEtiquetteSelonTag(cartonAPrendre);

        // 5. Mettre à jour l'UI d'interaction
        MettreAJourTexteUI();
    }

    void OuvrirEtiquetteSelonTag(GameObject carton)
    {
        Transform target = carton.transform;

        // Si le tag est sur le sous-objet / modèle 3D enfant
        if (carton.transform.childCount > 0)
        {
            Transform enfant = carton.transform.GetChild(0);
            if (enfant.CompareTag("Dangereux") || enfant.CompareTag("Boites"))
            {
                target = enfant;
            }
        }

        // Récupération automatique de sécurité si perdu
        if (etiquetteDangereuseUI == null) etiquetteDangereuseUI = GameObject.Find("ADR");
        if (etiquetteClassiqueUI == null) etiquetteClassiqueUI = GameObject.Find("Etiquette");

        // Activer l'objet existant dans le Canvas
        if (target.CompareTag("Dangereux"))
        {
            if (etiquetteDangereuseUI != null)
            {
                etiquetteDangereuseUI.SetActive(true);
                FermerEtiquette.estOuverte = true;
            }
        }
        else if (target.CompareTag("Boites"))
        {
            if (etiquetteClassiqueUI != null)
            {
                etiquetteClassiqueUI.SetActive(true);
                FermerEtiquette.estOuverte = true;
            }
        }
    }
}