using UnityEngine;
using System.Collections;
using TMPro;

public class InteractableExplosion : MonoBehaviour
{
    public enum State { Idle, Warning, Exploded, Respawning }
    public State currentState = State.Idle;

    [Header("Interaction")]
    public float interactRange = 6f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject pressEText; // ลาก Text UI มาใส่

    [Header("Explosion Settings")]
    public float shakeDuration   = 0.3f;
    public float explosionDelay  = 0.5f;
    public float explosionForce  = 10f;
    public float explosionRadius = 5f;

    [Header("Blast Collider")]
    public float blastDuration = 0.15f;

    [Header("Fragments")]
    public GameObject[] fragmentPrefabs;
    public int   fragmentCount    = 8;
    public float fragmentLifetime = 3f;

    [Header("Respawn")]
    public bool  autoRespawn  = true;
    public float respawnDelay = 5f;

    public Transform player;

    // ── Private ──────────────────────────────────────
    private Vector3    _startPos;
    private Quaternion _startRot;
    private bool       _triggered;
    private Renderer   _renderer;
    private GameObject _blastObject;
    private SphereCollider _blastCol;

    void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _renderer = GetComponentInChildren<Renderer>();

        // ซ่อน UI ตอนเริ่ม
        if (pressEText != null)
            pressEText.SetActive(false);

        // Blast Zone
        _blastObject = new GameObject("BlastZone");
        _blastObject.transform.SetParent(transform);
        _blastObject.transform.localPosition = Vector3.zero;
        _blastObject.tag = "Trap";

        _blastCol = _blastObject.AddComponent<SphereCollider>();
        _blastCol.radius = 0f;
        _blastCol.isTrigger = false;

        _blastObject.SetActive(false);
    }

    void Update()
    {
        if (currentState != State.Idle || _triggered)
        {
            HideUI();
            return;
        }

        bool inRange = PlayerInRange();

        // แสดง/ซ่อน UI
        if (pressEText != null)
            pressEText.SetActive(inRange);

        // กด E เพื่อระเบิด
        if (inRange && Input.GetKeyDown(interactKey))
        {
            HideUI();
            StartCoroutine(ExplosionRoutine());
        }
        Debug.Log("InRange: " + PlayerInRange());
    }

    void HideUI()
    {
        if (pressEText != null)
            pressEText.SetActive(false);
    }

    // ══════════════════════════════════════════════════
    //  CHECK PLAYER RANGE
    // ══════════════════════════════════════════════════
    bool PlayerInRange()
    {
        if (player == null) return false;

        return Vector3.Distance(transform.position, player.position) <= interactRange;
    }

    // ══════════════════════════════════════════════════
    //  MAIN ROUTINE
    // ══════════════════════════════════════════════════
    IEnumerator ExplosionRoutine()
    {
        _triggered   = true;
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

    // SHAKE
    IEnumerator Shake()
    {
        float elapsed  = 0f;
        float strength = 0.05f;

        while (elapsed < shakeDuration)
        {
            transform.position = _startPos + Random.insideUnitSphere * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPos;
    }

    // EXPLODE
    void Explode()
    {
        if (_renderer != null) _renderer.enabled = false;

        foreach (var col in GetComponents<Collider>())
            col.enabled = false;

        SpawnFragments();
        ApplyExplosionForce();
        StartCoroutine(BlastCollider());
    }

    IEnumerator BlastCollider()
    {
        _blastObject.SetActive(true);
        _blastCol.radius = explosionRadius;

        yield return new WaitForSeconds(blastDuration);

        _blastCol.radius = 0f;
        _blastObject.SetActive(false);
    }

    // FRAGMENTS
    void SpawnFragments()
    {
        if (fragmentPrefabs == null || fragmentPrefabs.Length == 0)
        {
            SpawnDefaultFragments();
            return;
        }

        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject prefab = fragmentPrefabs[Random.Range(0, fragmentPrefabs.Length)];
            GameObject frag = Instantiate(prefab, transform.position, Random.rotation);

            Rigidbody rb = frag.GetComponent<Rigidbody>() ?? frag.AddComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);

            Destroy(frag, fragmentLifetime);
        }
    }

    void SpawnDefaultFragments()
    {
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject frag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frag.transform.position = transform.position + Random.insideUnitSphere * 0.3f;
            frag.transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);

            Rigidbody rb = frag.AddComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);

            Destroy(frag, fragmentLifetime);
        }
    }

    void ApplyExplosionForce()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
        }
    }

    // RESPAWN
    void Respawn()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;

        if (_renderer != null) _renderer.enabled = true;

        foreach (var col in GetComponents<Collider>())
            col.enabled = true;

        _triggered   = false;
        currentState = State.Idle;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Gizmos.color = new Color(1f, 0.2f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}