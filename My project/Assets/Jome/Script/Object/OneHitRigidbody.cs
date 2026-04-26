using UnityEngine;

public class OneHitChaser : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Distance Settings")]
    public float detectRange = 5f;   // ระยะตรวจจับ
    public float stopRange = 1.5f;   // ระยะที่ถือว่า "ชน"

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Hit")]
    public float hitForce = 10f;

    private bool hasHit = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // อยู่ในระยะตรวจจับ และยังไม่เคยชน
        if (distance <= detectRange && !hasHit)
        {
            // วิ่งเข้า Player
            Vector3 dir = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + dir * moveSpeed * Time.fixedDeltaTime);

            // ถ้าใกล้พอ = ชน
            if (distance <= stopRange)
            {
                HitPlayer();
            }
        }
    }

    void HitPlayer()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            playerRb.AddForce(dir * hitForce, ForceMode.Impulse);

            hasHit = true; // ชนได้ครั้งเดียว
        }
    }

    // รีเซ็ตเมื่อ Player ออกระยะ
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            hasHit = false;
        }
    }
}