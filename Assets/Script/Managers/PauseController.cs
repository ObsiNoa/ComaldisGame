using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Panneaux à gérer")]
    [SerializeField] private GameObject optionsPanel; // Ton panneau d'options principal

    private bool isOpen = false;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.started += OnToggleOptions;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.started -= OnToggleOptions;
            pauseAction.action.Disable();
        }
    }

    private void OnToggleOptions(InputAction.CallbackContext context)
    {
        // Interdit l'ouverture/fermeture via Échap sur la scène MainMenu
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            return;

        ToggleOptionsMenu();
    }

    public void ToggleOptionsMenu()
    {
        isOpen = !isOpen;

        // Ouvre ou ferme le panneau d'options
        if (optionsPanel != null)
            optionsPanel.SetActive(isOpen);

        // Bloque ou débloque le temps de jeu (seul le MainMenu restera en temps réel)
        Time.timeScale = isOpen ? 0f : 1f;

        // Déverrouille la souris si le jeu était en vue 3D
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    // À associer au bouton "Fermer" ou "Retour" dans ton panneau d'options
    public void CloseMenu()
    {
        if (isOpen)
        {
            ToggleOptionsMenu();
        }
    }
}