using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RealisticFPS_Fix : MonoBehaviour
{
    [Header("Auto camera (ничего не назначай)")]
    public Transform cameraRoot;
    public Camera cam;

    [Header("Old Input")]
    public string axisX = "Horizontal";
    public string axisY = "Vertical";
    public string mouseX = "Mouse X";
    public string mouseY = "Mouse Y";
    public string jumpBtn = "Jump";
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Look")]
    public float mouseSens = 2.2f;
    public float pitchMin = -85f;
    public float pitchMax = 85f;
    public float lookSmooth = 18f;

    [Header("Speed (m/s)")]
    public float walkSpeed = 4.6f;
    public float sprintSpeed = 7.2f;
    public float crouchSpeed = 2.6f;

    [Header("Acceleration")]
    public float groundAccel = 30f;
    public float airAccel = 10f;
    public float friction = 14f;
    public float maxSlopeAngle = 55f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.15f;
    public float coyoteTime = 0.12f;
    public float jumpBuffer = 0.12f;
    public float extraGravity = 22f;
    public float stickToGroundForce = 10f;

    [Header("Ground Check")]
    public LayerMask groundMask = ~0;
    public float groundCheckDist = 0.28f;

    [Header("Crouch")]
    [Range(0.4f, 1f)] public float crouchHeightMul = 0.55f;
    public float crouchLerp = 12f;

    [Header("Camera feel")]
    public float baseFov = 75f;
    public float sprintFov = 83f;
    public float fovLerp = 8f;

    [Header("Headbob")]
    public float bobAmount = 0.06f;
    public float bobSpeedWalk = 12f;
    public float bobSpeedSprint = 16f;
    public float bobSpeedCrouch = 9f;

    [Header("Sway (mouse)")]
    public float swayAmount = 0.05f;
    public float swayMax = 0.08f;
    public float swayLerp = 10f;

    [Header("Strafe tilt")]
    public float strafeTiltDeg = 8f;
    public float tiltLerp = 8f;

    [Header("Landing kick")]
    public float landKick = 0.08f;
    public float landReturn = 14f;

    Rigidbody rb;
    CapsuleCollider capsule;

    float yaw, pitch;
    Vector2 smoothedMouse;

    bool grounded;
    bool wasGrounded;
    Vector3 groundNormal = Vector3.up;

    float lastGroundedTime;
    float lastJumpPressedTime;

    float standHeight, standCenterY;
    float targetHeight, targetCenterY;

    Vector3 camLocalDefaultPos;
    Vector3 camBobOffset;
    Vector3 camSwayOffset;
    float camTilt;
    float landingOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // --- ЖЁСТКО чиню RB, чтобы ты точно мог двигаться
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // ВАЖНО: не Freeze Position!

        // --- Камера: создаём и ставим "в голову" по капсуле
        SetupCameraAuto();

        standHeight = capsule.height;
        standCenterY = capsule.center.y;
        targetHeight = standHeight;
        targetCenterY = standCenterY;

        camLocalDefaultPos = cameraRoot.localPosition;

        if (cam) cam.fieldOfView = baseFov;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetupCameraAuto()
    {
        // создаём root
        var existingRoot = transform.Find("CameraRoot");
        if (!existingRoot)
        {
            var rootGO = new GameObject("CameraRoot");
            rootGO.transform.SetParent(transform);
            existingRoot = rootGO.transform;
        }

        cameraRoot = existingRoot;

        // "в голову": верх капсулы минус небольшой запас
        float headY = capsule.center.y + (capsule.height * 0.5f) - (capsule.radius * 0.9f);
        cameraRoot.localPosition = new Vector3(0f, headY, 0f);
        cameraRoot.localRotation = Quaternion.identity;

        // камера
        cam = GetComponentInChildren<Camera>(true);
        if (!cam)
        {
            var camGO = new GameObject("PlayerCamera");
            camGO.transform.SetParent(cameraRoot);
            camGO.transform.localPosition = Vector3.zero;
            camGO.transform.localRotation = Quaternion.identity;
            cam = camGO.AddComponent<Camera>();
        }
        else
        {
            // если камера была где-то в детях — запихнём её под CameraRoot
            cam.transform.SetParent(cameraRoot, true);
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        // --- Мышь (со сглаживанием)
        Vector2 rawMouse = new Vector2(Input.GetAxisRaw(mouseX), Input.GetAxisRaw(mouseY)) * mouseSens;
        smoothedMouse = Vector2.Lerp(smoothedMouse, rawMouse, 1f - Mathf.Exp(-lookSmooth * Time.deltaTime));

        yaw += smoothedMouse.x;
        pitch -= smoothedMouse.y;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (Input.GetButtonDown(jumpBtn))
            lastJumpPressedTime = Time.time;

        // --- Присед (hold)
        bool crouch = Input.GetKey(crouchKey);
        if (crouch)
        {
            targetHeight = standHeight * crouchHeightMul;
            targetCenterY = standCenterY * crouchHeightMul;
        }
        else
        {
            if (CanStandUp())
            {
                targetHeight = standHeight;
                targetCenterY = standCenterY;
            }
        }

        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * crouchLerp);
        Vector3 c = capsule.center;
        c.y = Mathf.Lerp(c.y, targetCenterY, Time.deltaTime * crouchLerp);
        capsule.center = c;

        // обновляем позицию головы при изменении высоты
        float headY = capsule.center.y + (capsule.height * 0.5f) - (capsule.radius * 0.9f);
        camLocalDefaultPos = new Vector3(0f, headY, 0f);
    }

    void FixedUpdate()
    {
        GroundCheck();
        if (grounded) lastGroundedTime = Time.time;

        float x = Input.GetAxisRaw(axisX);
        float z = Input.GetAxisRaw(axisY);

        // Реалистичнее: назад и вбок чуть медленнее
        float forwardMul = (z >= 0f) ? 1f : 0.75f;
        float strafeMul = 0.88f;

        Vector3 input = new Vector3(x * strafeMul, 0f, z * forwardMul);
        input = Vector3.ClampMagnitude(input, 1f);

        bool crouch = Input.GetKey(crouchKey);
        bool sprint = Input.GetKey(sprintKey) && !crouch && z > 0.1f;
        float targetSpeed = crouch ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);

        Vector3 wishDir = transform.TransformDirection(input);
        wishDir.y = 0f;
        wishDir = wishDir.sqrMagnitude > 0.0001f ? wishDir.normalized : Vector3.zero;

        if (grounded && wishDir != Vector3.zero)
            wishDir = Vector3.ProjectOnPlane(wishDir, groundNormal).normalized;

        Vector3 v = rb.linearVelocity;
        Vector3 horiz = new Vector3(v.x, 0f, v.z);
        Vector3 desired = wishDir * targetSpeed;

        float accel = grounded ? groundAccel : airAccel;
        Vector3 newHoriz = Vector3.MoveTowards(horiz, desired, accel * Time.fixedDeltaTime);

        if (grounded && input.sqrMagnitude < 0.0001f)
            newHoriz = Vector3.MoveTowards(newHoriz, Vector3.zero, friction * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(newHoriz.x, v.y, newHoriz.z);

        // прыжок (coyote + buffer)
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool buffered = (Time.time - lastJumpPressedTime) <= jumpBuffer;

        if (buffered && canCoyote)
        {
            lastJumpPressedTime = -999f;
            lastGroundedTime = -999f;

            float jumpVel = Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);
            Vector3 vv = rb.linearVelocity;
            if (vv.y < 0f) vv.y = 0f;
            vv.y += jumpVel;
            rb.linearVelocity = vv;
        }

        // “вес”
        if (!grounded) rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        else rb.AddForce(-groundNormal * stickToGroundForce, ForceMode.Acceleration);

        // landing kick
        if (!wasGrounded && grounded)
            landingOffset = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.y) * 0.02f) * landKick;

        wasGrounded = grounded;
    }

    void LateUpdate()
    {
        float x = Input.GetAxisRaw(axisX);
        float targetTilt = -x * strafeTiltDeg;
        camTilt = Mathf.Lerp(camTilt, targetTilt, 1f - Mathf.Exp(-tiltLerp * Time.deltaTime));

        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        bool crouch = Input.GetKey(crouchKey);
        bool sprint = Input.GetKey(sprintKey) && !crouch && Input.GetAxisRaw(axisY) > 0.1f;

        float bobSpd = crouch ? bobSpeedCrouch : (sprint ? bobSpeedSprint : bobSpeedWalk);
        float bobAmp = bobAmount * Mathf.InverseLerp(0f, sprintSpeed, speed) * (grounded ? 1f : 0.2f);

        float bob = Mathf.Sin(Time.time * bobSpd) * bobAmp;
        float bob2 = Mathf.Cos(Time.time * bobSpd * 2f) * bobAmp * 0.5f;
        camBobOffset = new Vector3(bob2, Mathf.Abs(bob), 0f);

        Vector3 targetSway = new Vector3(
            Mathf.Clamp(-smoothedMouse.x * swayAmount, -swayMax, swayMax),
            Mathf.Clamp(-smoothedMouse.y * swayAmount, -swayMax, swayMax),
            0f
        );
        camSwayOffset = Vector3.Lerp(camSwayOffset, targetSway, 1f - Mathf.Exp(-swayLerp * Time.deltaTime));

        landingOffset = Mathf.Lerp(landingOffset, 0f, 1f - Mathf.Exp(-landReturn * Time.deltaTime));

        cameraRoot.localPosition = camLocalDefaultPos + camBobOffset + camSwayOffset + (Vector3.down * landingOffset);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, camTilt);

        if (cam)
        {
            float targetFov = sprint ? sprintFov : baseFov;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-fovLerp * Time.deltaTime));
        }
    }

    void GroundCheck()
    {
        float radius = Mathf.Max(0.05f, capsule.radius - 0.02f);
        Vector3 origin = transform.position + Vector3.up * (capsule.radius + 0.02f);
        float castDist = groundCheckDist + 0.1f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, castDist, groundMask, QueryTriggerInteraction.Ignore))
        {
            groundNormal = hit.normal;
            float slope = Vector3.Angle(groundNormal, Vector3.up);
            grounded = slope <= maxSlopeAngle && hit.distance <= groundCheckDist + 0.12f;
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.up;
        }
    }

    bool CanStandUp()
    {
        float radius = capsule.radius - 0.01f;
        float standHeightNow = standHeight;

        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 topStand = transform.position + Vector3.up * (capsule.center.y + standHeightNow - radius);
        return !Physics.CheckCapsule(bottom, topStand, radius, groundMask, QueryTriggerInteraction.Ignore);
    }
}