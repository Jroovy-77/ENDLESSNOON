using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRigidBodyAnimator : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator anim;
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;

    [Header("Step Settings (ระบบขึ้นบันได)")]
    public float stepHeight = 0.4f;    // ความสูงของขั้นบันไดที่ก้าวข้ามได้
    public float stepSmooth = 0.1f;    // ความเร็วในการยกตัวขึ้น (ใช้น้อยๆ จะสมูท)
    public float stepReach = 0.1f;     // ระยะห่างจากตัวละครถึงขั้นบันได

    // ตัวแปรสำหรับเช็คพื้น (ยังคงไว้เพื่อให้ระบบ StepClimb หรือ Animation เดินทำงานได้ถูกต้อง)
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;
    private bool isGrounded;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        // ปรับ Collision Detection เป็น Continuous เพื่อความแม่นยำ
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        // เช็คพื้นเพื่อส่งค่าให้ Animator (ตัวละครจะได้ไม่เล่นท่าลอยตัว)
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
        
        if (anim != null) anim.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        MoveAndAnimate();
        
        // เรียกใช้ระบบขึ้นบันไดเฉพาะตอนที่มีแรงเคลื่อนที่ในแนวนอน
        if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 0.1f)
        {
            StepClimb();
        }
    }

    void MoveAndAnimate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // คำนวณทิศทางเดินตามหน้ากล้อง
        Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // ส่งค่าความเร็วให้ Animator
        if (anim != null) anim.SetFloat("Speed", moveDir.magnitude, 0.1f, Time.fixedDeltaTime);

        if (moveDir.magnitude > 0.1f)
        {
            // หันหน้าตัวละคร
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * turnSpeed));

            // เคลื่อนที่ด้วย Velocity
            Vector3 targetVelocity = moveDir * moveSpeed;
            targetVelocity.y = rb.velocity.y; // ปล่อยให้แรงโน้มถ่วงทำงานปกติ
            rb.velocity = targetVelocity;
        }
        else
        {
            // หยุดเดินแนวนอนทันทีเมื่อปล่อยปุ่ม
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    // 🔥 ระบบก้าวขึ้นบันไดอัตโนมัติ
    void StepClimb()
    {
        // ยิง Ray จากส่วนเท้าไปข้างหน้า
        if (Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), transform.forward, out RaycastHit hitLower, stepReach + 0.1f))
        {
            // ยิง Ray อีกเส้นในจุดที่สูงกว่า (stepHeight) เพื่อเช็คว่าเป็นขั้นบันไดหรือไม่
            if (!Physics.Raycast(transform.position + new Vector3(0, stepHeight, 0), transform.forward, out RaycastHit hitUpper, stepReach + 0.2f))
            {
                // ดันตัวละครขึ้นแกน Y ทีละนิดเพื่อให้ข้ามขอบวัตถุได้
                rb.position += new Vector3(0f, stepSmooth, 0f);
            }
        }
    }
}
