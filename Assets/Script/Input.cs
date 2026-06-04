using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Ne pas oublier pour détecter les Buttons

public class InputVN : MonoBehaviour
{
    public static InputVN Instance;
    public VNMode modeActuel;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (modeActuel != VNMode.Dialogue)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // On vérifie si l'objet survolé est un bouton ou un enfant de bouton (le texte du bouton)
                GameObject overObj = EventSystem.current.currentSelectedGameObject;
                if (overObj != null && (overObj.GetComponent<Button>() != null || overObj.GetComponentInParent<Button>() != null))
                {
                    return; // C'est un vrai bouton, on bloque le clic de dialogue
                }
            }

            // Si on arrive ici, c'est un clic normal dans le vide ou sur le texte : on passe à la ligne suivante
            DialogueManager.Instance.NextLine();
        }
    }
}
public enum VNMode
{
    Zone,
    Dialogue,
    Choix
}

