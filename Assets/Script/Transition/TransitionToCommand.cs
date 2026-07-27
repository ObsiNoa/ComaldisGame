using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionToCommand : MonoBehaviour
{
    [Header("Ref UI")]
    public TextMeshProUGUI txtUI;

    [Header("Changement de scène")]
    public string nomDeLaScene;

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
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            joueur = true;
            if (txtUI != null)
            {
                txtUI.text = "[E] Se garer";
                txtUI.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
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

        if (string.IsNullOrEmpty(nomDeLaScene))
        {
            Debug.LogWarning("Aucun nom de scène renseigné dans l'inspecteur !");
            return;
        }

        SceneManager.LoadScene(nomDeLaScene);
    }
}