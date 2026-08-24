using UnityEngine;
using UnityEngine.InputSystem;

public class BindingManager: MonoBehaviour
{
    [Header("Référence à ton Asset d'Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject ActualPanel;

    public void ResetAllBindings()
    {
        // 1. Réinitialise toutes les actions aux touches par défaut
        if (inputActions != null)
        {
            inputActions.RemoveAllBindingOverrides();
        }

        // 2. Efface la sauvegarde locale des touches
        PlayerPrefs.DeleteKey("rebinds");
        PlayerPrefs.Save();

        // 3. Rafraîchit l'affichage de tous les boutons du menu
        var rebindUIComponents = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var comp in rebindUIComponents)
        {
            if (comp.GetType().Name == "RebindActionUI")
            {
                comp.Invoke("UpdateBindingDisplay", 0f);
            }
        }
    }

    public void Retour()
    {
        if (ActualPanel != null)
            ActualPanel.SetActive(false);

        if (MainPanel != null)
            MainPanel.SetActive(true);
    }
}