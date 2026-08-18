using System.Collections.Generic;
using UnityEngine;

public class BoxRow : MonoBehaviour
{
    [Header("Ordre des cartons (du plus proche au plus éloigné)")]
    public List<GameObject> boxes = new List<GameObject>();

    private void Start()
    {
       
        if (boxes.Count == 0)
        {
            foreach (Transform child in transform)
            {
                boxes.Add(child.gameObject);
            }
        }
    }

    // <summary>
    //Vérifie si le carton visé/cliqué est bien le 1er disponible.
    // </summary>
    public bool IsFrontBox(GameObject targetBox)
    {
        return boxes.Count > 0 && boxes[0] == targetBox;
    }

    // <summary>
    // Retire le premier carton de la liste et le renvoie.
    // </summary>
    public GameObject TakeFrontBox()
    {
        if (boxes.Count == 0) return null;

        GameObject takenBox = boxes[0];
        boxes.RemoveAt(0); // Le 2ème carton devient automatiquement le 1er (index 0) !

        return takenBox;
    }
}