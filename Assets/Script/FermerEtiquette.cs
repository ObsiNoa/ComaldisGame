using UnityEngine;
using UnityEngine.UI;

public class FermerEtiquette : MonoBehaviour
{
    // Permet à n'importe quel script du jeu de savoir si une étiquette est affichée
    public static bool estOuverte = false;

    [Header("Référence Bouton UI")]
    public Button boutonFermer;

    private VNMode previousMode;
    private CursorLockMode previousCursorState;
    private bool previousCursorVisible;

    void Start()
    {
        // 1. On signale que l'étiquette est ouverte
        estOuverte = true;

        // 2. Sauvegarder l'état du VN
        if (InputVN.Instance != null)
        {
            previousMode = InputVN.Instance.modeActuel;
            InputVN.Instance.modeActuel = VNMode.Zone;
        }

        // 3. Afficher et libérer le curseur de la souris
        previousCursorState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4. Mettre en pause les animations/physiques
        Time.timeScale = 0f;

        // 5. Écouter le bouton de fermeture
        if (boutonFermer != null)
        {
            boutonFermer.onClick.AddListener(FermerLEtiquette);
        }
    }

    public void FermerLEtiquette()
    {
        // 1. Relancer le temps
        Time.timeScale = 1f;

        // 2. Remettre le curseur comme avant
        Cursor.lockState = previousCursorState;
        Cursor.visible = previousCursorVisible;

        // 3. Restaurer le mode de jeu
        if (InputVN.Instance != null)
        {
            InputVN.Instance.modeActuel = previousMode;
        }

        // 4. On signale que l'étiquette est fermée
        estOuverte = false;

        // 5. Détruire l'étiquette
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Sécurité si l'objet est détruit autrement
        estOuverte = false;
        Time.timeScale = 1f;
    }
}