using UnityEngine;
using System.Collections;

public class TrapRoller : MonoBehaviour
{
    public enum State { Idle, Rolling, Returning, Cooldown }
    public State currentState = State.Idle;

    [Header("Detection (Cone)")]
    public float detectionDistance = 10f;
    public float detectionAngle    = 60f;

    [Header("Roll Settings")]
    public float rollSpeed     = 12f;
    public float rollDistance  = 15f;
    public float rotationSpeed = 360f;

    [Header("Return Settings")]
    public float returnSpeed = 4f;
    public float cooldown    = 3f;

    private Vector3    _startPos;
    private Quaternion _startRot;
    private Vector3    _rollDir;
    private bool       _hitSomething;

    void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    void Update()
    {
        if (currentState != State.Idle) return;
        if (DetectPlayer(out Vector3 playerPos))
            StartCoroutine(RollRoutine(playerPos));
    }

    // ── Detection ───────────────────────────────────
    bool DetectPlayer(out Vector3 playerPos)
    {
        playerPos = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;

        Vector3 toPlayer = player.transform.position - transform.position;
        float   dist     = toPlayer.magnitude;
        float   angle    = Vector3.Angle(transform.forward, toPlayer);

        if (dist <= detectionDistance && angle <= detectionAngle * 0.5f)
        {
            playerPos = player.transform.position;
            return true;
        }
        return false;
    }

    // ── Main Routine ────────────────────────────────
    IEnumerator RollRoutine(Vector3 playerPos)
    {
        _rollDir = (playerPos - transform.position).normalized;
        _rollDir.y = 0f;
        _hitSomething = false;

        currentState = State.Rolling;
        yield return StartCoroutine(Roll());

        yield return new WaitForSeconds(cooldown);

        currentState = State.Returning;
        yield return StartCoroutine(ReturnToStart());

        currentState = State.Cooldown;
        yield return new WaitForSeconds(1f);

        currentState = State.Idle;
    }

    // ── Roll (Transform ล้วน ไม่ใช้ OverlapSphere) ──
    IEnumerator Roll()
    {
        float traveled = 0f;

        while (traveled < rollDistance && !_hitSomething)
        {
            float   step    = rollSpeed * Time.deltaTime;
            Vector3 rotAxis = Vector3.Cross(Vector3.up, _rollDir);

            transform.position += _rollDir * step;
            transform.Rotate(rotAxis, rotationSpeed * Time.deltaTime, Space.World);
            traveled += step;

            yield return null;
        }
    }

    // ── ชนอะไรก็หยุด (OnCollisionEnter จัดการ) ─────
    void OnCollisionEnter(Collision col)
    {
        if (currentState != State.Rolling) return;
        if (col.gameObject == gameObject)  return;

        _hitSomething = true;

        // TODO: ดาเมจ Player
        // if (col.gameObject.CompareTag("Player"))
        //     col.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(20);

        Debug.Log("ชน: " + col.gameObject.name);
    }

    // ── Return ──────────────────────────────────────
    IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.position, _startPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, _startPos, returnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, _startRot, 90f * Time.deltaTime);
            yield return null;
        }
        transform.position = _startPos;
        transform.rotation = _startRot;
    }

    // ── Gizmos ──────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        int   segments  = 20;
        float halfAngle = detectionAngle * 0.5f * Mathf.Deg2Rad;

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
        for (int i = 0; i <= segments; i++)
        {
            float t     = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 dirH = transform.rotation *
                new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * detectionDistance;
            Gizmos.DrawRay(transform.position, dirH);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * detectionDistance);

        Vector3 coneEnd    = transform.position + transform.forward * detectionDistance;
        float   coneRadius = Mathf.Tan(halfAngle) * detectionDistance;
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f);
        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i       / segments * Mathf.PI * 2f;
            float a2 = (float)(i + 1) / segments * Mathf.PI * 2f;
            Vector3 p1 = coneEnd + transform.rotation *
                new Vector3(Mathf.Cos(a1), Mathf.Sin(a1), 0f) * coneRadius;
            Vector3 p2 = coneEnd + transform.rotation *
                new Vector3(Mathf.Cos(a2), Mathf.Sin(a2), 0f) * coneRadius;
            Gizmos.DrawLine(p1, p2);
        }
    }
}