using System.Collections;
using UnityEngine;

public class PauseZone : MonoBehaviour
{
    [Header("UI & Timing")]
    public GameObject panelUI;          // Glisse ton Panel ici
    public GameObject texteAAfficher;   // Glisse ton texte (ex: Text_1èreZone) ici
    public float duree = 3f;

    [Header("Joueur")]
    public MonoBehaviour playerMovement; // Ton script de déplacement

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

        // Active le Panel ET le Texte
        if (panelUI != null)
            panelUI.SetActive(true);

        if (texteAAfficher != null)
            texteAAfficher.SetActive(true);

        // Gèle le jeu
        Time.timeScale = 0f;

        // Attend les secondes réelles
        yield return new WaitForSecondsRealtime(duree);

        // Reprend le jeu
        Time.timeScale = 1f;

        // Désactive le Panel ET le Texte à la fin du temps
        if (panelUI != null)
            panelUI.SetActive(false);

        if (texteAAfficher != null)
            texteAAfficher.SetActive(false);

        // Réactive les mouvements
        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}