using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FastFallingTrap : MonoBehaviour
{
    private Rigidbody rb;
    private bool isActivated = false;

    [Header("Detection")]
    public Transform target;           // ลาก Player มาใส่ (หรือจะใช้ Tag หาเอง)
    public float detectionRange = 7f;  // ระยะเริ่มทำงาน

    [Header("Speed & Force")]
    public float downForce = 25f;      // แรงถีบลง (ยิ่งเยอะยิ่งพุ่งเร็ว)
    public float dashForce = 15f;      // แรงพุ่งไปหาตัวผู้เล่นในแนวนอน
    public float gravityMultiplier = 40f; // แรงดึงดูดจำลอง (ทำให้ตกเร็วแบบหนักๆ)

    [Header("Collision Layers")]
    public string groundLayer = "Ground";
    public string playerLayer = "Player";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        // ปรับเป็น Continuous เพื่อกันทะลุพื้นเมื่อตกเร็ว
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void Update()
    {
        if (isActivated || target == null) return;

        // เช็คระยะห่างระหว่าง "หิน" กับ "คน"
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= detectionRange)
        {
            ActivateTrap();
        }
    }

    void FixedUpdate()
    {
        if (isActivated)
        {
            // เพิ่มแรงดึงดูดพิเศษลงไปตลอดเวลาเพื่อให้ตกเร็วขึ้นเรื่อยๆ
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    public void ActivateTrap()
    {
        if (isActivated) return;
        isActivated = true;

        // 1. ตั้งค่าให้ทะลุทุกอย่าง ยกเว้นพื้นกับคน
        SetupCollision();

        // 2. ปล่อยฟิสิกส์
        rb.isKinematic = false;
        rb.useGravity = true;

        // 3. คำนวณทิศทางพุ่งใส่คน
        Vector3 dashDir = (target.position - transform.position).normalized;
        dashDir.y = 0; // เอาแค่ทิศทางแนวนอน

        // รวมแรง (ลงล่าง + พุ่งไปหาคน)
        Vector3 totalForce = (Vector3.down * downForce) + (dashDir * dashForce);
        rb.AddForce(totalForce, ForceMode.Impulse);
    }

    void SetupCollision()
    {
        int gLayer = LayerMask.NameToLayer(groundLayer);
        int pLayer = LayerMask.NameToLayer(playerLayer);
        int myLayer = gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if (i != gLayer && i != pLayer)
                Physics.IgnoreLayerCollision(myLayer, i, true);
        }
    }

    // วาดวงกลมระยะตรวจจับให้ดูในหน้า Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}