using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoubleDoorUnlockGlobal : MonoBehaviour
{
    public Transform leftHinge;
    public Transform rightHinge;

    public KeyCode interactKey = KeyCode.E;

    [Header("Open")]
    public float openAngle = 95f;
    public float degreesPerSecond = 180f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Debug")]
    public bool debugLogs = true;

    private bool _playerInside;
    private bool _open;

    private Quaternion _leftClosed, _rightClosed;
    private Quaternion _leftTarget, _rightTarget;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // чтобы триггер работал стабильно
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (!audioSource) audioSource = GetComponent<AudioSource>();

        if (!leftHinge || !rightHinge)
        {
            Debug.LogError($"[{name}] Assign leftHinge/rightHinge!", this);
            enabled = false;
            return;
        }

        _leftClosed = leftHinge.localRotation;
        _rightClosed = rightHinge.localRotation;
        UpdateTargets();

        if (debugLogs)
            Debug.Log($"[{name}] Door ready. Needs key: HasDoorKey={GameFlags.HasDoorKey}", this);
    }

    private void Update()
    {
        if (_playerInside && Input.GetKeyDown(interactKey))
        {
            if (debugLogs)
                Debug.Log($"[{name}] E pressed. HasDoorKey={GameFlags.HasDoorKey}", this);

            if (!GameFlags.HasDoorKey)
            {
                if (debugLogs) Debug.Log($"[{name}] LOCKED: key not picked.", this);
            }
            else
            {
                _open = !_open;
                UpdateTargets();

                if (audioSource)
                {
                    var clip = _open ? openClip : closeClip;
                    if (clip) audioSource.PlayOneShot(clip, volume);
                }

                if (debugLogs) Debug.Log($"[{name}] TOGGLE open={_open}", this);
            }
        }

        float step = degreesPerSecond * Time.deltaTime;
        leftHinge.localRotation  = Quaternion.RotateTowards(leftHinge.localRotation, _leftTarget, step);
        rightHinge.localRotation = Quaternion.RotateTowards(rightHinge.localRotation, _rightTarget, step);
    }

    private void UpdateTargets()
    {
        var leftOpen  = _leftClosed  * Quaternion.Euler(0f, -openAngle, 0f);
        var rightOpen = _rightClosed * Quaternion.Euler(0f,  openAngle, 0f);

        _leftTarget  = _open ? leftOpen : _leftClosed;
        _rightTarget = _open ? rightOpen : _rightClosed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        if (debugLogs) Debug.Log($"[{name}] ENTER door zone", this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        if (debugLogs) Debug.Log($"[{name}] EXIT door zone", this);
    }
}
