using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsRush : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Movement Settings")]
    public float speed = 20f;
    public float stoppingDistance = 1.5f; 

    private Rigidbody rb;
    private bool hasStruck = false; // ตัวเช็คว่าชน/เข้าระยะแล้วหรือยัง

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; 

        // หา Player อัตโนมัติถ้าไม่ได้ใส่ไว้
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) player = go.transform;
        }
    }

    void FixedUpdate()
    {
        // ถ้าไม่มีเป้าหมาย หรือ "ชนไปแล้ว" ให้หยุดการทำงานทั้งหมด
        if (player == null || hasStruck) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stoppingDistance)
        {
            // --- ช่วงที่กำลังพุ่ง ---
            Vector3 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
            transform.LookAt(player);
        }
        else
        {
            // --- จังหวะที่เข้าระยะชนครั้งแรก ---
            FinalStop();
        }
    }

    // เมื่อวัตถุไปกระแทกอะไรบางอย่าง (เช่น ตัว Player)
    private void OnCollisionEnter(Collision collision)
    {
        // ถ้าชนสิ่งที่มี Tag เดียวกับที่เราตามหา ให้หยุดทันที
        if (collision.gameObject.CompareTag(playerTag))
        {
            FinalStop();
        }
    }

    void FinalStop()
    {
        hasStruck = true; // ล็อคเป้าว่าชนแล้ว ห้ามพุ่งอีก
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // ปิดระบบฟิสิกส์เพื่อไม่ให้มันไถลหรือกระดอน
        Debug.Log("Hit and Stopped!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}