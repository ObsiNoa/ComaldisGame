using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI & Panneaux")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Joueur & Caméra")]
    [Tooltip("Glisse ici le composant Player Input de ton Joueur/Capsule")]
    [SerializeField] private GameObject playerInput;

    // Si tu utilises un script de caméra de type FirstPersonController sans PlayerInput :
    // [SerializeField] private MonoBehaviour cameraScript;

    private bool isOpen = false;

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

        // 1. Affichage du menu
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(isOpen);
        }

        // 2. Blocage des entrées / scripts du Joueur
        if (playerInput != null)
        {
            // Tente de couper le PlayerInput s'il existe
            PlayerInput pi = playerInput.GetComponent<PlayerInput>();
            if (pi != null)
            {
                pi.enabled = !isOpen;
            }

            // Tente de couper un script de contrôleur classique s'il y en a un
            MonoBehaviour controller = playerInput.GetComponent<MonoBehaviour>();
            if (controller != null && pi == null)
            {
                controller.enabled = !isOpen;
            }
        }

        // 3. Gestion du temps et du curseur
        Time.timeScale = isOpen ? 0f : 1f;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    public void CloseMenu()
    {
        if (isOpen)
        {
            ToggleOptionsMenu();
        }
    }
}