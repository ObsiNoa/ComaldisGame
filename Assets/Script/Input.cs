using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

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
                    return; 
                }
            }

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

