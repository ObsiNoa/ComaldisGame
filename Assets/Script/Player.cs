using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Transform cameraTransform;

    public float speed = 5f;
    public float sensibility = 0.5f;
    public float rotationX = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked; // Cache la souris
    }


    void Update()
    {  // Ancienne version 

        //// 1. ROTATIONS (Souris)
        //// Gauche/Droite : On tourne le corps du joueur directement
        //transform.Rotate(0, Input.GetAxis("Mouse X") * sensibility, 0);

        //// Haut/Bas : On stocke la rotation, on la limite, et on l'applique à la caméra
        //rotationX -= Input.GetAxis("Mouse Y") * sensibility;
        //rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        //cameraTransform.localEulerAngles = new Vector3(rotationX, 0, 0); 

        //// 2. DÉPLACEMENTS (Clavier ZQSD)
        //Vector3 deplacement = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        //// Application du mouvement + une petite gravité constante de -2f pour rester au sol
        //controller.Move((deplacement * speed + Vector3.down * 2f) * Time.deltaTime);

        // 1. ROTATIONS (Souris avec le Nouveau Système)
        if (Pointer.current != null)
        {
            Vector2 deltaSouris = Mouse.current.delta.ReadValue();

            // Gauche/Droite
            transform.Rotate(0, deltaSouris.x * sensibility, 0);

            // Haut/Bas
            rotationX -= deltaSouris.y * sensibility;
            rotationX = Mathf.Clamp(rotationX, -80f, 80f);
            cameraTransform.localEulerAngles = new Vector3(rotationX, 0, 0);
        }

        // 2. DÉPLACEMENTS (Clavier ZQSD avec le Nouveau Système)
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) z = 1f; // Z ou Flèche Haut
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z = -1f; // S ou Flèche Bas
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f; // Q ou Flèche Gauche
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f; // D ou Flèche Droite
        }

        Vector3 deplacement = transform.right * x + transform.forward * z;

        // Application du mouvement + gravité
        controller.Move((deplacement * speed + Vector3.down * 2f) * Time.deltaTime);
    }
}

