using UnityEngine;
using System.Collections;

public class ExplodeOnNear : MonoBehaviour
{
    public enum State { Idle, Warning, Exploded, Respawning }
    public State currentState = State.Idle;

    [Header("Player")]
    public Transform player;
    public float detectRange = 4f;

    [Header("Explosion")]
    public float shakeDuration = 0.3f;
    public float explosionDelay = 0.3f;
    public float explosionForce = 10f;
    public float explosionRadius = 5f;

    [Header("Fragments")]
    public GameObject[] fragmentPrefabs;
    public int fragmentCount = 8;
    public float fragmentLifetime = 3f;

    [Header("Respawn")]
    public bool autoRespawn = true;
    public float respawnDelay = 5f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Renderer rend;
    private bool triggered;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        rend = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (player == null || triggered) return;

        if (currentState == State.Idle && IsPlayerNear())
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    bool IsPlayerNear()
    {
        return Vector3.Distance(transform.position, player.position) <= detectRange;
    }

    IEnumerator ExplosionRoutine()
    {
        triggered = true;
        currentState = State.Warning;

        yield return StartCoroutine(Shake());
        yield return new WaitForSeconds(explosionDelay);

        Explode();
        currentState = State.Exploded;

        if (autoRespawn)
        {
            currentState = State.Respawning;
            yield return new WaitForSeconds(respawnDelay);
            Respawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Shake()
    {
        float t = 0f;
        float strength = 0.05f;
        Vector3 original = startPos;

        while (t < shakeDuration)
        {
            transform.position = original + Random.insideUnitSphere * strength;
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = original;
    }

    void Explode()
    {
        if (rend != null) rend.enabled = false;

        foreach (var col in GetComponents<Collider>())
            col.enabled = false;

        SpawnFragments();
        ApplyExplosionForce();
    }

    void SpawnFragments()
    {
        if (fragmentPrefabs != null && fragmentPrefabs.Length > 0)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject prefab = fragmentPrefabs[Random.Range(0, fragmentPrefabs.Length)];
                GameObject frag = Instantiate(prefab, transform.position, Random.rotation);

                Rigidbody rb = frag.GetComponent<Rigidbody>() ?? frag.AddComponent<Rigidbody>();
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                Destroy(frag, fragmentLifetime);
            }
        }
    }

    void ApplyExplosionForce()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var col in cols)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    void Respawn()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        if (rend != null) rend.enabled = true;

        foreach (var col in GetComponents<Collider>())
            col.enabled = true;

        triggered = false;
        currentState = State.Idle;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}