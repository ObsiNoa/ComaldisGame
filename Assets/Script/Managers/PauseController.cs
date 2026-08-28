using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI & Panneaux")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Joueur")]
    [Tooltip("Glisse ici directement ton GameObject Joueur (Capsule)")]
    [SerializeField] private GameObject playerGameObject;

    private bool isOpen = false;

    // Sauvegarde de l'état de la souris avant l'ouverture du menu
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    private void Start()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.actionMap.Enable();
            pauseAction.action.Enable();
            pauseAction.action.started += OnPauseInputTriggered;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.started -= OnPauseInputTriggered;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TryToggleMenu();
        }
    }

    private void OnPauseInputTriggered(InputAction.CallbackContext context)
    {
        TryToggleMenu();
    }

    private void TryToggleMenu()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            return;

        ToggleOptionsMenu();
    }

    public void ToggleOptionsMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            // --- OUVERTURE DU MENU ---
            
            // 1. On sauvegarde l'état actuel du curseur (3D ou 2D)
            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;

            // 2. On affiche le menu
            if (optionsPanel != null) optionsPanel.SetActive(true);

            // 3. On libère le curseur pour l'UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 4. On gère le temps et le joueur
            Time.timeScale = 0f;
            SetPlayerInputActive(false);
        }
        else
        {
            // --- FERMETURE DU MENU ---

            // 1. On masque le menu
            if (optionsPanel != null) optionsPanel.SetActive(false);

            // 2. On restaure l'état exact du curseur d'AVANT l'ouverture
            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;

            // 3. On réactive le temps et le joueur
            Time.timeScale = 1f;
            SetPlayerInputActive(true);
        }
    }

    private void SetPlayerInputActive(bool active)
    {
        if (playerGameObject == null) return;

        // Désactive/Active l'Input System du Joueur
        PlayerInput pi = playerGameObject.GetComponent<PlayerInput>();
        if (pi != null)
        {
            pi.enabled = active;
            return;
        }

        // Sinon désactive/active le premier contrôleur trouvé
        MonoBehaviour controller = playerGameObject.GetComponent<MonoBehaviour>();
        if (controller != null)
        {
            controller.enabled = active;
        }
    }

    public void CloseMenu()
    {
        if (isOpen)
        {
            ToggleOptionsMenu();
        }
    }
}