using System.Collections;
using UnityEngine;

public class PauseZone : MonoBehaviour
{
    public GameObject canvas;
    public MonoBehaviour playerMovement; // Ton script de déplacement
    public float duree = 3f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;
        StartCoroutine(Pause());
    }

    IEnumerator Pause()
    {
        // Empêche le joueur de bouger
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Affiche le message
        canvas.SetActive(true);

        // Gèle le jeu
        Time.timeScale = 0f;

        // Attend 3 secondes réelles
        yield return new WaitForSecondsRealtime(duree);

        // Reprend le jeu
        Time.timeScale = 1f;

        canvas.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}