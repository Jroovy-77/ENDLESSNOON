using UnityEngine;
using System.Collections;

public class TrapLauncher : MonoBehaviour
{
    [Header("Player (ลากใส่เอง)")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDistance = 5f;

    [Header("Return Settings")]
    public float returnSpeed = 3f;
    public float cooldown = 2f;

    private Vector3 _startPos;
    private bool _ready = true;
    private Collider[] _ignoredAtStart;

    void Start()
    {
        _startPos = transform.position;

        // จำ Collider รอบข้างตอนเริ่ม
        _ignoredAtStart = Physics.OverlapSphere(transform.position, 0.5f);
    }

    void Update()
    {
        if (!_ready) return;
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            StartCoroutine(TrapRoutine(player.position));
        }
    }

    IEnumerator TrapRoutine(Vector3 playerPos)
    {
        _ready = false;

        yield return StartCoroutine(Dash(playerPos));

        yield return new WaitForSeconds(cooldown);

        yield return StartCoroutine(ReturnToStart());

        _ready = true;
    }

    IEnumerator Dash(Vector3 playerPos)
    {
        Vector3 dir = (playerPos - transform.position).normalized;
        Vector3 target = transform.position + dir * dashDistance;

        float traveled = 0f;

        while (traveled < dashDistance)
        {
            float step = dashSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                step
            );

            traveled += step;

            if (HitSomething()) break;

            yield return null;
        }
    }

    bool HitSomething()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.4f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            bool wasIgnored = false;

            foreach (var ignored in _ignoredAtStart)
            {
                if (hit == ignored)
                {
                    wasIgnored = true;
                    break;
                }
            }

            if (!wasIgnored) return true;
        }

        return false;
    }

    IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.position, _startPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _startPos,
                returnSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = _startPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}