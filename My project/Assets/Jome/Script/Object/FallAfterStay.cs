using UnityEngine;

public class FallAfterStay : MonoBehaviour
{
    [Header("Target")]
    public Transform player;          // ลาก Player มาใส่
    public string playerTag = "Player";

    [Header("Settings")]
    public float triggerRange = 3f;   // ระยะตรวจจับ
    public float requiredTime = 4f;   // เวลาที่ต้องยืนในระยะ

    private float stayTimer = 0f;
    private bool hasFallen = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ตอนแรกยังไม่ตก
    }

    void Update()
    {
        if (hasFallen || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // เช็คว่า Player อยู่ในระยะ
        if (distance <= triggerRange)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= requiredTime)
            {
                Fall();
            }
        }
        else
        {
            // ออกจากระยะ รีเซ็ตเวลา
            stayTimer = 0f;
        }
    }

    void Fall()
    {
        rb.isKinematic = false;
        hasFallen = true;
    }

    // วาดวงระยะใน Scene ให้เห็นเลย
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}