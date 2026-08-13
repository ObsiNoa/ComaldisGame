using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftInventory : MonoBehaviour
{
    [Header("Point d'attache du carton sur les fourches")]
    public Transform pointCarton;

    [Header("Ajustement Modèle Carton")]
    [Tooltip("Correction de rotation appliquée (ex: Y = 90, Z = 90)")]
    public Vector3 rotationOffset = new Vector3(0, 90, 90);

    [Header("Configuration Dépose")]
    [Tooltip("Layer du sol pour détecter la hauteur exacte de pose.")]
    public LayerMask solLayer;

    [Tooltip("Hauteur supplémentaire pour le poser sur la palette.")]
    public float hauteurOffsetDepot = 0.65f;

    public GameObject cartonActuel = null;
    public bool AUnCarton => cartonActuel != null;

    private bool vientDEtreAttache = false;

    void Update()
    {
        if (FermerEtiquette.estOuverte) return;

        if (vientDEtreAttache)
        {
            vientDEtreAttache = false;
            return;
        }

        // Touche [A] (physique sur AZERTY) pour poser au sol
        if (AUnCarton)
        {
            if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
            {
                PoserCartonSurLeSol();
            }
        }
    }

    public void AttacherCarton(GameObject carton)
    {
        cartonActuel = carton;
        vientDEtreAttache = true;

        carton.transform.SetParent(pointCarton);
        carton.transform.localPosition = Vector3.zero;
        carton.transform.localRotation = Quaternion.Euler(rotationOffset);

        Collider col = carton.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = carton.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void PoserCartonSurLeSol()
    {
        if (cartonActuel == null) return;

        GameObject cartonDepose = cartonActuel;
        Quaternion rotationTransport = cartonDepose.transform.rotation;

        Collider col = cartonDepose.GetComponent<Collider>();

        // 1. Réactiver D'ABORD le collider pour que Unity calcule ses vraies dimensions
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        // 2. Obtenir la vraie demi-hauteur (avec valeur de secours si besoin)
        float demiHauteur = (col != null && col.bounds.extents.y > 0.05f) ? col.bounds.extents.y : 0.25f;

        // 3. Tirer le Raycast vers le sol depuis un point légèrement plus haut
        Vector3 origineRaycast = pointCarton.position + Vector3.up * 1.0f;
        Vector3 positionDepot;

        // Si solLayer n'est pas configuré dans l'Inspector, on prend toutes les collisions
        int layerMask = (solLayer.value != 0) ? solLayer.value : ~0;

        if (Physics.Raycast(origineRaycast, Vector3.down, out RaycastHit hit, 10f, layerMask))
        {
            // Point d'impact au sol + demi-hauteur du carton + offset palette
            positionDepot = hit.point + new Vector3(0, demiHauteur + hauteurOffsetDepot, 0);
        }
        else
        {
            // Sécurité si aucun sol n'est détecté
            positionDepot = pointCarton.position + new Vector3(0, hauteurOffsetDepot, 0);
        }

        // 4. Déposer le carton à la bonne position et rotation
        cartonDepose.transform.SetParent(null);
        cartonDepose.transform.position = positionDepot;
        cartonDepose.transform.rotation = rotationTransport;

        // 5. Réactiver la physique
        Rigidbody rb = cartonDepose.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // 6. Réactiver le composant de ramassage au sol
        CartonRamassable ramassable = cartonDepose.GetComponent<CartonRamassable>();
        if (ramassable == null)
        {
            ramassable = cartonDepose.AddComponent<CartonRamassable>();
        }
        ramassable.enabled = true;

        cartonActuel = null;
    }
}