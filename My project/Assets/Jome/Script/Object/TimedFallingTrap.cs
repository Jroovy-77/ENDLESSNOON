using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TimedFallingTrap : MonoBehaviour
{
    private Rigidbody rb;
    private bool isActivated = false;
    private float stayTimer = 0f; // ตัวนับเวลา

    [Header("Detection Settings")]
    public Transform target;           // ลาก Player มาใส่
    public float detectionRange = 5f;  // ระยะตรวจจับ
    public float timeThreshold = 5f;   // ต้องยืนแช่นานกี่วินาที (ตั้งไว้ 5)

    [Header("Fall Speed")]
    public float downForce = 30f;      // แรงพุ่งลงตอนเริ่ม
    public float gravityMultiplier = 40f; // แรงดึงดูดพิเศษ

    [Header("Collision Layers")]
    public string groundLayer = "Ground";
    public string playerLayer = "Player";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
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

        // 1. เช็คระยะห่าง
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= detectionRange)
        {
            // 2. ถ้าอยู่ในระยะ ให้เริ่มนับเวลาเพิ่มขึ้นเรื่อยๆ
            stayTimer += Time.deltaTime;
            Debug.Log("ตรวจพบเป้าหมาย: " + stayTimer.ToString("F1") + " วินาที");

            // 3. ถ้าถึงกำหนด (5 วิ) ให้ทำงาน
            if (stayTimer >= timeThreshold)
            {
                ActivateTrap();
            }
        }
        else
        {
            // 4. ถ้าเดินออกจากระยะก่อน 5 วิ ให้รีเซ็ตเวลาเป็น 0 (หรือจะไม่รีเซ็ตก็ได้แล้วแต่ดีไซน์)
            stayTimer = 0f; 
        }
    }

    void FixedUpdate()
    {
        if (isActivated)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void ActivateTrap()
    {
        isActivated = true;
        
        // ตั้งค่าให้ทะลุเพดาน/หินอื่น
        SetupCollision();

        rb.isKinematic = false;
        rb.useGravity = true;

        // ดีดลงมาอย่างแรง
        rb.AddForce(Vector3.down * downForce, ForceMode.Impulse);
        Debug.Log("ยืนแช่นานเกินไป! หินหล่นทับ!");
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

    private void OnDrawGizmosSelected()
    {
        // วาดรัศมีตรวจจับสีเขียว
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}