using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionToDashboard : MonoBehaviour
{
    [Header("Ref UI")]
    public TextMeshProUGUI txtUI;

    [Header("Changement de scène")]
    public string nomDeLaScene;

    [Header("Effets Visuels")]
    public ParticleSystem Particules;

    private bool joueur = false;

    void Update()
    {
        if (joueur)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ChangerDeScene();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player" && Particules != null)
        {
            joueur = true;
            if (txtUI != null)
            {
                txtUI.text = "[E] Monter dans le camion";
                txtUI.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player" && Particules != null)
        {
            joueur = false;
            if (txtUI != null)
            {
                txtUI.gameObject.SetActive(false);
            }
        }
    }

    void ChangerDeScene()
    {
        Debug.Log("CHANGEMENT DE SCÈNE : " + nomDeLaScene);

        if (Particules != null)
        {
            Particules.Stop();
        }

        if (string.IsNullOrEmpty(nomDeLaScene))
        {
            Debug.LogWarning("Aucun nom de scène renseigné dans l'inspecteur !");
            return;
        }

        SceneManager.LoadScene(nomDeLaScene);
    }
}