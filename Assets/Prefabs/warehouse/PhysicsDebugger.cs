using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsDebugger : MonoBehaviour
{
    Rigidbody rb;

    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Debug.Log("=== PHYSICS DEBUG START ===");
        Debug.Log("Time.timeScale = " + Time.timeScale);
        Debug.Log("AutoSimulation = " + Physics.autoSimulation);
        Debug.Log("Is Kinematic = " + rb.isKinematic);
        Debug.Log("Use Gravity = " + rb.useGravity);
        Debug.Log("Constraints = " + rb.constraints);
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        // раз в 1 секунду лог
        if (timer >= 1f)
        {
            timer = 0f;

            Debug.Log(
                "Velocity: " + rb.linearVelocity +
                " | Position: " + transform.position
            );
        }
    }

    void Update()
    {
        // тест принудительного движения
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("FORCE MOVE TEST");
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
    }
}