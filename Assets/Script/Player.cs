using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Transform cameraTransform;

    public float speed = 5f;
    public float sensibility = 0.5f;
    public float rotationX = 0f;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;

    private void Awake()
    {
        // On récupère l'action Move depuis l'Asset
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
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. ROTATIONS (Souris)
        if (Pointer.current != null)
        {
            Vector2 deltaSouris = Mouse.current.delta.ReadValue();

            transform.Rotate(0, deltaSouris.x * sensibility, 0);

            rotationX -= deltaSouris.y * sensibility;
            rotationX = Mathf.Clamp(rotationX, -80f, 80f);
            cameraTransform.localEulerAngles = new Vector3(rotationX, 0, 0);
        }

        // 2. DÉPLACEMENTS (Utilise l'action Move rebindable)
        Vector2 inputVector = moveAction.ReadValue<Vector2>(); // Récupère un Vector2 (X = Gauche/Droite, Y = Avant/Arrière)

        Vector3 deplacement = transform.right * inputVector.x + transform.forward * inputVector.y;

        controller.Move((deplacement * speed + Vector3.down * 2f) * Time.deltaTime);
    }
}