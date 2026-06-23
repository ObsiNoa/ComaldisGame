using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class ZoneInteractions : MonoBehaviour
{
    [Header("Config")]
    public string nomDeLaVerification;

    [Header("Ref UI")]
    public TextMeshProUGUI texteUI;

    [Header("Lien Check-list")]
    public GestionnaireCheckList checklist; 
    [Tooltip("Écrire 'pneus' ou 'bache'")]
    public string idTacheChecklist;

    private bool joueurDansLaZone = false;
    private bool verificationFaite = false; 

    void Start()
    {
        
    }

  
    void Update()
    {
        if (joueurDansLaZone && !verificationFaite)
        {
            if(Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ValiderVerification(); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            joueurDansLaZone = true;

            if (!verificationFaite && texteUI != null)
            {
                // Construit le message : "[E] pour vérifier la bâche"
                texteUI.text = "[E] " + " " + nomDeLaVerification;
                texteUI.gameObject.SetActive(true); // Affiche le texte à l'écran
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
                texteUI.gameObject.SetActive(false); // Cache le texte quand le joueur s'éloigne
            }
        }
    }

    void ValiderVerification()
    {
        verificationFaite = true;
        Debug.Log("VÉRIFICATION RÉUSSIE : " + nomDeLaVerification);

        if (checklist != null)
        {
            checklist.CocherTache(idTacheChecklist);
        }
        if (texteUI != null)
        {
            // Optionnel : change le texte pour montrer que c'est validé avant de le cacher
            texteUI.text = nomDeLaVerification + " Vérifié !";
            // On cache le texte après 1 seconde pour laisser le joueur voir la validation
            Invoke("CacherUI", 1f);
        }
    }

    void CacherUI()
    {
        if (texteUI != null) texteUI.gameObject.SetActive(false);
    }
}
