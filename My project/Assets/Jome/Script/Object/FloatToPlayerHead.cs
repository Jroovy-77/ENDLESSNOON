using UnityEngine;

public class FloatToPlayerHead : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Range")]
    public float detectRange = 5f;

    [Header("Follow Settings")]
    public float heightOffset = 2f;
    public float followSpeed = 5f;

    [Header("Timer")]
    public float followDuration = 5f; // เวลาที่เกาะหัว

    [Header("Fade")]
    public float fadeSpeed = 2f; // ความเร็วในการจาง

    private bool isFollowing = false;
    private bool isFading = false;

    private float followTimer = 0f;

    private Rigidbody rb;
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // เข้าใกล้ → เริ่มเกาะหัว
        if (distance <= detectRange && !isFollowing && !isFading)
        {
            isFollowing = true;
            followTimer = 0f;
        }

        if (isFollowing)
        {
            followTimer += Time.deltaTime;

            FollowPlayerHead();

            if (followTimer >= followDuration)
            {
                isFollowing = false;
                isFading = true;
            }
        }

        if (isFading)
        {
            FadeOut();
        }
    }

    void FollowPlayerHead()
    {
        Vector3 targetPos = player.position + Vector3.up * heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    void FadeOut()
    {
        if (rend == null) return;

        Color c = rend.material.color;
        c.a -= fadeSpeed * Time.deltaTime;
        rend.material.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // วาดระยะ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}