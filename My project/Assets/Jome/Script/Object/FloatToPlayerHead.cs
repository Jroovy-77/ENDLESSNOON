using UnityEngine;

public class FloatToPlayerHead : MonoBehaviour
{
    [Header("Target")]
    public Transform player;        // ลาก Player มาใส่

    [Header("Range")]
    public float detectRange = 5f;  // ระยะตรวจจับ

    [Header("Follow Settings")]
    public float heightOffset = 2f; // สูงเหนือหัว
    public float followSpeed = 5f;  // ความเร็วลอยตาม

    private bool isFollowing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // เข้าใกล้ → เริ่มลอยตาม
        if (distance <= detectRange)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            FollowPlayerHead();
        }
    }

    void FollowPlayerHead()
    {
        // เป้าหมาย = เหนือหัว Player
        Vector3 targetPos = player.position + Vector3.up * heightOffset;

        // เคลื่อนที่แบบลื่น ๆ
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // กันตก/กันฟิสิกส์รบกวน
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    // วาดเส้นระยะใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}