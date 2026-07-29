using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwitchAreaAndTeleportTrigger : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;                 // корень игрока (с CharacterController)
    public string playerTag = "Player";

    [Header("Teleport")]
    public Transform teleportTarget;             // куда телепортировать
    public bool keepPlayerYawFromTarget = true;  // повернуть игрока как у teleportTarget

    [Header("Objects to Disable/Enable")]
    public GameObject[] disableObjects;          // что выключить
    public GameObject[] enableObjects;           // что включить

    [Header("One-shot")]
    public bool triggerOnce = true;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool _used;

    private void Awake()
    {
        // гарантируем триггер
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Если playerRoot не задан — найдём по тегу
        if (!playerRoot)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go) playerRoot = go.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && _used) return;
        if (!other.CompareTag(playerTag)) return;

        _used = true;

        // 1) Выключаем
        for (int i = 0; i < disableObjects.Length; i++)
        {
            if (disableObjects[i]) disableObjects[i].SetActive(false);
        }

        // 2) Включаем
        for (int i = 0; i < enableObjects.Length; i++)
        {
            if (enableObjects[i]) enableObjects[i].SetActive(true);
        }

        // 3) Телепорт
        if (playerRoot && teleportTarget)
        {
            // Если игрок управляется CharacterController — на время отключаем,
            // чтобы позиция применилась без "отката"
            CharacterController cc = playerRoot.GetComponent<CharacterController>();
            bool hadCC = cc != null;

            if (hadCC) cc.enabled = false;

            playerRoot.position = teleportTarget.position;

            if (keepPlayerYawFromTarget)
            {
                Vector3 e = teleportTarget.rotation.eulerAngles;
                playerRoot.rotation = Quaternion.Euler(0f, e.y, 0f);
            }

            if (hadCC) cc.enabled = true;
        }

        if (debugLogs)
            Debug.Log($"[{name}] Triggered: switched objects + teleported.", this);
    }
}
