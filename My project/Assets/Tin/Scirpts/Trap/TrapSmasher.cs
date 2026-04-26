using UnityEngine;
using System.Collections;

public class TrapSmasher : MonoBehaviour
{
    public enum State { Idle, Attacking, Sinking, Respawning }
    public State currentState = State.Idle;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("Attack Settings")]
    public float attackSpeed = 20f;

    [Header("Sink Settings")]
    public float holdTime  = 0.75f;
    public float sinkSpeed = 3f;
    public float sinkDepth = 3f;

    [Header("Respawn Settings")]
    public bool  autoRespawn  = true;
    public float respawnDelay = 4f;

    // ── Private ──────────────────────────────────────
    private Vector3 _startPos;
    private Vector3 _targetPos;

    // ── Unity ────────────────────────────────────────
    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        if (currentState != State.Idle) return;
        if (DetectPlayer(out Vector3 playerPos))
            StartCoroutine(AttackRoutine(playerPos));
    }

    // ══════════════════════════════════════════════════
    //  DETECTION
    // ══════════════════════════════════════════════════
    bool DetectPlayer(out Vector3 playerPos)
    {
        playerPos = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= detectionRange)
        {
            playerPos = player.transform.position;
            return true;
        }
        return false;
    }

    // ══════════════════════════════════════════════════
    //  MAIN ROUTINE
    // ══════════════════════════════════════════════════
    IEnumerator AttackRoutine(Vector3 playerPos)
    {
        // ล็อกตำแหน่ง Player ตอนเริ่ม
        _targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.z);

        currentState = State.Attacking;
        yield return StartCoroutine(Attack());

        yield return new WaitForSeconds(holdTime);

        currentState = State.Sinking;
        yield return StartCoroutine(Sink());

        if (autoRespawn)
        {
            currentState = State.Respawning;
            yield return new WaitForSeconds(respawnDelay);
            Respawn();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // ══════════════════════════════════════════════════
    //  ATTACK — พุ่งไปทับ
    // ══════════════════════════════════════════════════
    IEnumerator Attack()
    {
        while (Vector3.Distance(transform.position, _targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, _targetPos, attackSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _targetPos;
    }

    // ══════════════════════════════════════════════════
    //  SINK — มุดลงพื้น
    // ══════════════════════════════════════════════════
    IEnumerator Sink()
    {
        Vector3 sinkTarget = transform.position + Vector3.down * sinkDepth;

        while (Vector3.Distance(transform.position, sinkTarget) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, sinkTarget, sinkSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = sinkTarget;
    }

    // ══════════════════════════════════════════════════
    //  RESPAWN — กลับจุดเริ่ม
    // ══════════════════════════════════════════════════
    void Respawn()
    {
        transform.position = _startPos;
        currentState       = State.Idle;
    }

    // ══════════════════════════════════════════════════
    //  DAMAGE
    // ══════════════════════════════════════════════════
    void OnCollisionEnter(Collision col)
    {
        if (currentState != State.Attacking) return;
        if (!col.gameObject.CompareTag("Player")) return;

        // TODO: ใส่ดาเมจตรงนี้
        // col.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(30);
        Debug.Log("ทับ Player!");
    }

    // ══════════════════════════════════════════════════
    //  GIZMOS
    // ══════════════════════════════════════════════════
    void OnDrawGizmosSelected()
    {
        // ระยะตรวจจับ
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // แสดงทิศทางมุดลง
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.down * sinkDepth);
    }
}