using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftInventory : MonoBehaviour
{
    [Header("Point d'attache du carton sur les fourches")]
    public Transform pointCarton;

    [Header("Configuration Dépose")]
    [Tooltip("Layer du sol pour détecter la hauteur exacte de pose.")]
    public LayerMask solLayer;

    // Référence vers le carton actuellement transporté (null si vide)
    public GameObject cartonActuel = null;

    /// <summary>
    /// Indique si le transpalette transporte déjà un carton
    /// </summary>
    public bool AUnCarton => cartonActuel != null;

    void Update()
    {
        // Sécurité : si une étiquette est ouverte à l'écran, on bloque le lâcher
        if (FermerEtiquette.estOuverte) return;

        // Si on transporte un carton et qu'on appuie sur [E]
        if (AUnCarton)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                PoserCartonSurLeSol();
            }
        }
    }

    /// <summary>
    /// Attache un carton sur les fourches du Forklift
    /// </summary>
    public void AttacherCarton(GameObject carton)
    {
        cartonActuel = carton;

        // Parenter au ForkPoint
        carton.transform.SetParent(pointCarton);
        carton.transform.localPosition = Vector3.zero;
        carton.transform.localRotation = Quaternion.identity;

        // Désactiver la physique et le collider pour ne pas bloquer la conduite
        Collider col = carton.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = carton.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    /// <summary>
    /// Dépose le carton n'importe où sur le sol à l'emplacement actuel des fourches
    /// </summary>
    public void PoserCartonSurLeSol()
    {
        if (cartonActuel == null) return;

        // 1. Calcul de la demi-hauteur
        float demiHauteur = 0.25f;
        Collider col = cartonActuel.GetComponent<Collider>();
        if (col != null)
        {
            demiHauteur = col.bounds.extents.y;
        }

        // 2. Localiser le sol
        Vector3 positionDepot = pointCarton.position;
        if (Physics.Raycast(pointCarton.position, Vector3.down, out RaycastHit hit, 3f, solLayer))
        {
            positionDepot = hit.point + new Vector3(0, demiHauteur, 0);
        }

        // 3. Positionner et détacher le carton
        GameObject cartonDepose = cartonActuel;
        cartonDepose.transform.SetParent(null);
        cartonDepose.transform.position = positionDepot;
        cartonDepose.transform.rotation = Quaternion.Euler(0, pointCarton.eulerAngles.y, 0);

        // 4. Réactiver physique et collisions
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; // S'assurer que la collision physique reste active
        }

        Rigidbody rb = cartonDepose.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // 5. AJOUT : Ajouter ou réactiver le script d'interaction individuelle
        CartonRamassable ramassable = cartonDepose.GetComponent<CartonRamassable>();
        if (ramassable == null)
        {
            ramassable = cartonDepose.AddComponent<CartonRamassable>();
        }
        else
        {
            ramassable.enabled = true;
        }
        cartonActuel = null;
    }
}