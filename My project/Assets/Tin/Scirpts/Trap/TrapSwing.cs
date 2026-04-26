using UnityEngine;
using System.Collections;

public class TrapSwing : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;
    public float returnSpeed   = 60f;

    [Header("Timing")]
    public float holdTime = 1f;
    public float cooldown = 3f;

    private bool       _ready    = true;
    private Quaternion _startRot;

    void Start()
    {
        _startRot = transform.rotation; // จำมุมเริ่มต้นไว้
    }

    void Update()
    {
        if (!_ready) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            StartCoroutine(SwingRoutine(player.transform.position));
    }

    IEnumerator SwingRoutine(Vector3 playerPos)
    {
        _ready = false;

        // คำนวณมุมที่ต้องหมุนไปหา Player
        Vector3    dir       = (playerPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        yield return StartCoroutine(RotateTo(targetRot, rotationSpeed));
        yield return new WaitForSeconds(holdTime);
        yield return StartCoroutine(RotateTo(_startRot, returnSpeed));
        yield return new WaitForSeconds(cooldown);

        _ready = true;
    }

    IEnumerator RotateTo(Quaternion target, float speed)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = target;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // TODO: other.GetComponent<PlayerHealth>()?.TakeDamage(10);
        Debug.Log("โดน Player!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}