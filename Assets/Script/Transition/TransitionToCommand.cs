using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionToCommand : MonoBehaviour
{
    [Header("Ref UI")]
    public TextMeshProUGUI txtUI;

    [Header("Changement de scène")]
    public string nomDeLaScene = "Livraison"; // Nom de la scène cible

    [Header("Effets Visuels")]
    public ParticleSystem Particules;

    private bool joueurDansLaZone = false;

    void Update()
    {
        if (joueurDansLaZone)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ChangerDeScene();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si c'est le joueur/camion via le Tag "Player" OU si l'objet contient "Camion" ou "Player" dans son nom
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Camion") || other.gameObject.name.Contains("Player"))
        {
            Debug.Log("---> QUELQUE CHOSE A TOUCHÉ LA ZONE : " + other.gameObject.name + " (Tag: " + other.tag + ")");
            joueurDansLaZone = true;

            if (txtUI != null)
            {
                txtUI.text = "[E] Se garer";
                txtUI.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Camion") || other.gameObject.name.Contains("Player"))
        {
            joueurDansLaZone = false;

            if (txtUI != null)
            {
                txtUI.gameObject.SetActive(false);
            }
        }
    }

    void ChangerDeScene()
    {
        Debug.Log("CHANGEMENT DE SCÈNE VERS : " + nomDeLaScene);

        if (Particules != null)
        {
            Particules.Stop();
        }

        if (string.IsNullOrEmpty(nomDeLaScene))
        {
            Debug.LogError("Erreur : Aucun nom de scène renseigné dans l'inspecteur !");
            return;
        }

        SceneLoader.SceneToLoad = nomDeLaScene;
        SceneManager.LoadScene("LoadingScreen");
    }
}