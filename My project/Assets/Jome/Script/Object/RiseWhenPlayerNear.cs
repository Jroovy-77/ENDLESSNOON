using UnityEngine;

public class RiseWhenPlayerNear : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // ลาก Player มาใส่

    [Header("Range")]
    public float detectRange = 5f; // ระยะตรวจจับ

    [Header("Rise Settings")]
    public float riseHeight = 3f;  // ความสูงที่โผล่ขึ้น
    public float riseSpeed = 3f;   // ความเร็วในการขึ้น

    private Vector3 startPos;
    private Vector3 targetPos;

    private bool isRising = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // เข้าใกล้ → เริ่มโผล่
        if (distance <= detectRange)
        {
            isRising = true;
        }

        if (isRising)
        {
            // ลอยขึ้นแบบลื่น ๆ
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                riseSpeed * Time.deltaTime
            );
        }
    }

    // วาดเส้นระยะใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}