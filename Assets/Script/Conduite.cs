using UnityEngine;
using UnityEngine.InputSystem;

public class Conduite : MonoBehaviour
{
    private CharacterController controller;
    private Transform cameraTransform;

    [Header("Paramètres du Poids")]
    public float maxForwardSpeed = 15f;
    public float maxReverseSpeed = 5f;
    public float acceleration = 3f;
    public float brakeForce = 12f;
    public float friction = 1.5f;
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
        if (Pointer.current != null)
        {
            Vector2 deltaSouris = Mouse.current.delta.ReadValue();

            rotationY += deltaSouris.x * sensibility;
            rotationY = Mathf.Clamp(rotationY, -60f, 60f);

            rotationX -= deltaSouris.y * sensibility;
            rotationX = Mathf.Clamp(rotationX, -15f, 15f);

            cameraTransform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }

        // Lecture des entrées rebindables (Vector2)
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

        float speedFactor = Mathf.Clamp(speed / maxForwardSpeed, -1f, 1f);
        float turnAmount = x * turnSpeed * speedFactor * Time.deltaTime;
        transform.Rotate(0f, turnAmount, 0f);

        Vector3 mouvementAvantArriere = transform.forward * speed;
        controller.Move((mouvementAvantArriere + Vector3.down * 2f) * Time.deltaTime);
    }
}