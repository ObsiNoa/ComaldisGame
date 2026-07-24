using System.Collections;
using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    public GameObject messageCanvas;
    public float duree = 5f;

    private bool dejaDeclenche = false;

    private void OnTriggerEnter(Collider other)
    {
        if (dejaDeclenche)
            return;

        if (other.CompareTag("Player"))
        {
            dejaDeclenche = true;
            StartCoroutine(AfficherMessage());
        }
    }

    IEnumerator AfficherMessage()
    {
        // Affiche le message
        messageCanvas.SetActive(true);

        // Met le jeu en pause
        Time.timeScale = 0f;

        // Attend 5 secondes réelles
        yield return new WaitForSecondsRealtime(duree);

        // Cache le message
        messageCanvas.SetActive(false);

        // Reprend le jeu
        Time.timeScale = 1f;
    }
}