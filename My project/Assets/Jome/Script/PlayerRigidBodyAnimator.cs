using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // บังคับว่า Object นี้ต้องมี Rigidbody
public class PlayerRigidBodyAnimator : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator anim;               // ลาก Animator มาใส่ที่นี่
    public Transform cameraTransform;   // ลาก Main Camera มาใส่ที่นี่

    [Header("Movement")]
    public float moveSpeed = 5f;        // ความเร็วเดิน
    public float turnSpeed = 15f;       // ความเร็วในการหันหน้า

    void Start()
    {
        // รับค่า Rigidbody ถ้ายังไม่ได้ลากใส่
        if (rb == null) rb = GetComponent<Rigidbody>();

        // 🔥 สำคัญมาก: ล็อคไม่ให้ Rigidbody ล้มคว่ำ
        // เราจะ freeze rotation แกน X และ Z ไว้ ให้หมุนได้แค่แกน Y (หันซ้าย-ขวา)
        rb.freezeRotation = true; 
    }

    void FixedUpdate()
    {
        // การคำนวณฟิสิกส์ต้องทำใน FixedUpdate
        MoveAndAnimate();
    }

    void MoveAndAnimate()
    {
        // 1. รับค่า Input (W,A,S,D)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. คำนวณทิศทางเดินอิงจากหน้ากล้อง (ตัดแกน Y ออก)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // ทิศทางที่ผู้เล่นต้องการไปจริงๆ
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 🔥 3. ส่งค่าไปยัง Animator
        // ต้องไปสร้าง Parameter ชื่อ "Speed" แบบ Float ใน Animator นะครับ
        if (anim != null)
        {
            // moveDir.magnitude จะเป็น 0 ถ้ายืนนิ่ง และเป็น 1 ถ้าเดิน
            // 0.1f คือค่า damping ช่วยให้เปลี่ยนท่าไม่กระตุก
            anim.SetFloat("Speed", moveDir.magnitude, 0.1f, Time.fixedDeltaTime);
        }

        // 4. ถ้ามีการกดปุ่มเดิน
        if (moveDir.magnitude > 0.1f)
        {
            // 4.1 หมุนตัวละคร: หันหน้าไปทิศที่เดินทันที
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * turnSpeed));

            // 4.2 การเคลื่อนที่: ใช้ Velocity เพื่อให้ความเร็วคงที่
            Vector3 targetVelocity = moveDir * moveSpeed;
            targetVelocity.y = rb.velocity.y; // รักษาความเร็วแกน Y (แรงโน้มถ่วง) เผื่อตกตึก
            rb.velocity = targetVelocity;
        }
        else
        {
            // ถ้าไม่กดเดิน ให้หยุดเดินในแนวนอนทันที (ตัวจะได้ไม่สไลด์)
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}