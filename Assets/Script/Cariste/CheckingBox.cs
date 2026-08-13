using System.Collections.Generic;
using UnityEngine;



public class ZoneNonDangereuse : MonoBehaviour
{
    [SerializeField] private GlobalCanvasManager canvasManager;

    public GameObject myObject;


    private void OnTriggerEnter(Collider other)
    {

        Transform lastChild = myObject.transform.GetChild(myObject.transform.childCount - 1);

        if (lastChild.childCount > 0)
        {
            Transform target = lastChild.GetChild(0);

            if (target.CompareTag("Dangereux"))
            {
                return;
            }
            else if (target.CompareTag("Boites"))
            {
                canvasManager.AjouterBoite();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        Transform lastChild = myObject.transform.GetChild(myObject.transform.childCount - 1);

        if (lastChild.childCount > 0)
        {
            Transform target = lastChild.GetChild(0);

            if (target.CompareTag("Dangereux"))
            {
                return;
            }
            else if (target.CompareTag("Boites"))
            {
                canvasManager.RetirerBoite();
            }
        }
    }
}


