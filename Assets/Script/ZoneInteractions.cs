using UnityEngine;
using UnityEngine.InputSystem;


public class ZoneInteractions : MonoBehaviour
{
    [Header("Config")]
    public string nomDeLaVerification;
    public string messageAffichage = "Appuyer sur E pour vérifier";

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

    // Détection : Le joueur entre dans la zone
    private void OnTriggerEnter(Collider other)
    {
        // On vérifie que c'est bien le joueur (la capsule) qui entre
        if (other.gameObject.name == "Player" || other.CompareTag("Player"))
        {
            joueurDansLaZone = true;
            if (!verificationFaite)
            {
                // Ici tu pourras plus tard relier ton UI pour afficher le texte à l'écran
                Debug.Log(messageAffichage + " : " + nomDeLaVerification);
            }
        }
    }

    // Détection : Le joueur sort de la zone
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" || other.CompareTag("Player"))
        {
            joueurDansLaZone = false;
            Debug.Log("Vous vous éloignez de : " + nomDeLaVerification);
        }
    }

    // Action de validation
    void ValiderVerification()
    {
        verificationFaite = true;
        Debug.Log("VÉRIFICATION RÉUSSIE : " + nomDeLaVerification);

        // C'est ici qu'on pourra déclencher un son, changer la couleur d'une icône 
        // ou ouvrir un dialogue/image comme on l'a fait au tout début !
    }
}
