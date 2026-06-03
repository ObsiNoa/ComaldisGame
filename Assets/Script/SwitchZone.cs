using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    private Stack<GameObject> historique = new Stack<GameObject>();
    public GameObject canvasActuel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvasActuel.SetActive(true);

        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Zone;
    }

    public bool PeutRetourner()
    {
        return historique != null && historique.Count > 0;
    }

    public void AllerVers(GameObject nouveauCanvas)
    {
        if (canvasActuel != null && canvasActuel != nouveauCanvas)
        {
            historique.Push(canvasActuel);
        }

        if (canvasActuel != null)
            canvasActuel.SetActive(false);

        canvasActuel = nouveauCanvas;
        canvasActuel.SetActive(true);
    }

    public void Retour()
    {
        if (historique.Count == 0) return;

        canvasActuel.SetActive(false);
        canvasActuel = historique.Pop();
        canvasActuel.SetActive(true);
    }
}

public class BoutonRetourUI : MonoBehaviour
{
    public Button boutonRetour;

    void Start()
    {
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        if (CanvasManager.Instance == null) return;

        boutonRetour.interactable = CanvasManager.Instance.PeutRetourner();
    }

    public void MettreAJourBouton(Button btn)
    {
        btn.interactable = CanvasManager.Instance.PeutRetourner();
    }
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue()
    {
        InputVN.Instance.modeActuel = VNMode.Dialogue;
    }

    public void NextLine()
    {
        Debug.Log("Ligne suivante");
    }

    public void StopDialogue()
    {
        InputVN.Instance.modeActuel = VNMode.Zone;
    }
}

public enum VNState
{
    Dialogue,
    Choix,
    Transition
}

public enum VNMode
{
    Zone,
    Dialogue
}

public class InputVN : MonoBehaviour
{
    public static InputVN Instance;

    public VNMode modeActuel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (InputVN.Instance != null)
            InputVN.Instance.modeActuel = VNMode.Zone;

    }

    void Update()
    {
        if (modeActuel != VNMode.Dialogue)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            DialogueManager.Instance.NextLine();
        }
    }
}

