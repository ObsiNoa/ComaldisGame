using UnityEngine;
using UnityEngine.InputSystem;

public class ConduiteForklift : MonoBehaviour
{
    private CharacterController controller;
    private Transform cameraTransform;

    [Header("Paramètres du Poids")]
    public float maxForwardSpeed = 15f;   // Vitesse max en avant
    public float maxReverseSpeed = 5f;    // Vitesse max en arrière (plus lente !)
    public float acceleration = 3f;       // Accélération lente pour simuler le poids
    public float brakeForce = 12f;        // Force des freins (quand on s'oppose au mouvement)
    public float friction = 1.5f;         // Frein moteur (quand on lâche toutes les touches)
    public float turnSpeed = 45f;

    private float speed = 0f;

    public float sensibility = 0.5f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;

    private void Awake()
    {
        // Récupération de l'action Move depuis l'asset
        moveAction = inputActions.FindAction("Move");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked; // Cache la souris
    }

    void Update()
    {
        // 1. ROTATIONS (Caméra / Souris)
        if (Pointer.current != null)
        {
            Vector2 deltaSouris = Mouse.current.delta.ReadValue();

            // Yaw (horizontal)
            rotationY += deltaSouris.x * sensibility;
            rotationY = Mathf.Clamp(rotationY, -60f, 60f);

            // Pitch (vertical)
            rotationX -= deltaSouris.y * sensibility;
            rotationX = Mathf.Clamp(rotationX, -15f, 15f);

            cameraTransform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }

        // 2. DÉPLACEMENTS (Utilise l'action Move rebindable)
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        float x = inputVector.x;
        float z = inputVector.y;

        bool pressingForward = z > 0.1f;
        bool pressingBackward = z < -0.1f;

        float targetSpeed = 0f;
        float currentChangeRate = friction;

        if (pressingForward)
        {
            if (speed < -0.1f) { targetSpeed = 0f; currentChangeRate = brakeForce; }
            else { targetSpeed = maxForwardSpeed; currentChangeRate = acceleration; }
        }
        else if (pressingBackward)
        {
            if (speed > 0.1f) { targetSpeed = 0f; currentChangeRate = brakeForce; }
            else { targetSpeed = -maxReverseSpeed; currentChangeRate = acceleration; }
        }

        speed = Mathf.MoveTowards(speed, targetSpeed, currentChangeRate * Time.deltaTime);

        // Rotation et déplacement du transpalette
        float speedFactor = Mathf.Clamp(speed / maxForwardSpeed, -1f, 1f);
        float turnAmount = x * turnSpeed * speedFactor * Time.deltaTime;
        transform.Rotate(0f, turnAmount, 0f);

        Vector3 mouvementAvantArriere = transform.forward * speed;

        controller.Move((mouvementAvantArriere + Vector3.down * 2f) * Time.deltaTime);
    }
}