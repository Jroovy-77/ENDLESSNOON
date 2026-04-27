using UnityEngine;
using System.Collections;

public class TrapSmasher : MonoBehaviour
{
    public enum State { Idle, Attacking, Sinking, Respawning }
    public State currentState = State.Idle;

    [Header("Player (ลากใส่เอง)")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("Attack Settings")]
    public float attackSpeed = 20f;

    [Header("Sink Settings")]
    public float holdTime = 0.75f;
    public float sinkSpeed = 3f;
    public float sinkDepth = 3f;

    [Header("Respawn Settings")]
    public bool autoRespawn = true;
    public float respawnDelay = 4f;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        if (currentState != State.Idle) return;
        if (player == null) return;

        if (DetectPlayer(out Vector3 playerPos))
        {
            StartCoroutine(AttackRoutine(playerPos));
        }
    }

    bool DetectPlayer(out Vector3 playerPos)
    {
        playerPos = Vector3.zero;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            playerPos = player.position;
            return true;
        }

        return false;
    }

    IEnumerator AttackRoutine(Vector3 playerPos)
    {
        _targetPos = new Vector3(
            playerPos.x,
            transform.position.y,
            playerPos.z
        );

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

    IEnumerator Attack()
    {
        while (Vector3.Distance(transform.position, _targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPos,
                attackSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = _targetPos;
    }

    IEnumerator Sink()
    {
        Vector3 sinkTarget = transform.position + Vector3.down * sinkDepth;

        while (Vector3.Distance(transform.position, sinkTarget) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                sinkTarget,
                sinkSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = sinkTarget;
    }

    void Respawn()
    {
        transform.position = _startPos;
        currentState = State.Idle;
    }

    void OnCollisionEnter(Collision col)
    {
        if (currentState != State.Attacking) return;
        if (player == null) return;

        if (col.transform == player)
        {
            Debug.Log("ทับ Player!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.down * sinkDepth);
    }
}