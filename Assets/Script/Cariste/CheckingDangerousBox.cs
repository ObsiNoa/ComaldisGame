using System.Collections.Generic;
using UnityEngine;


public class ZoneDangereuse : MonoBehaviour
{
    [SerializeField] private GlobalCanvasManager canvasManager;

    [Header("Parents autorisés")]
    [SerializeField] private Transform parent1;
    [SerializeField] private Transform parent2;
    [SerializeField] private Transform parent3;

    private void OnTriggerEnter(Collider other)
    {
        bool appartientAuxParents =
        other.transform.IsChildOf(parent1) ||
        other.transform.IsChildOf(parent2) ||
        other.transform.IsChildOf(parent3);

        if (!appartientAuxParents)
            return;

        // Ignore les objets dangereux
        if (!other.CompareTag("Dangereux"))
            return;

        canvasManager.AjouterBoite();
    }

    private void OnTriggerExit(Collider other)
    {
        bool appartientAuxParents =
            other.transform.IsChildOf(parent1) ||
            other.transform.IsChildOf(parent2) ||
            other.transform.IsChildOf(parent3);

        if (!appartientAuxParents)
            return;

        if (!other.CompareTag("Dangereux"))
            return;

        canvasManager.RetirerBoite();
    }
}