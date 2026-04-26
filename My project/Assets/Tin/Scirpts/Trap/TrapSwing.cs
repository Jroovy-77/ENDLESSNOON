using UnityEngine;
using System.Collections;

public class TrapSwing : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Settings")]
    public float tiltSpeed = 300f;   // ความเร็วตอนล้มลง
    public float spinSpeed = 1000f;  // ความเร็วตอนหมุนกวาด (ต้องเร็วๆ ถึงจะเท่)
    public float cooldown = 2f;

    private bool _ready = true;
    private Quaternion _startRot;

    void Start()
    {
        _startRot = transform.rotation;
    }

    void Update()
    {
        if (!_ready) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            StartCoroutine(TreeTrapRoutine());
        }
    }

    IEnumerator TreeTrapRoutine()
    {
        _ready = false;

        // --- STEP 1: ล้มลงแนวตั้ง (Tilt) ---
        // สมมติว่าล้มไปข้างหน้า 90 องศา (ปรับแกน X เป็น 90)
        Quaternion flatRot = Quaternion.Euler(90f, transform.eulerAngles.y, transform.eulerAngles.z);
        yield return StartCoroutine(MoveRotation(flatRot, tiltSpeed));

        // --- STEP 2: หมุนกวาดแนวนอน 360 องศา (Spin) ---
        // เราจะไม่ใช้ RotateTowards หา 360 ตรงๆ แต่จะใช้การบวกค่ามุมไปเรื่อยๆ แทน
        float angleSpun = 0f;
        while (angleSpun < 360f)
        {
            float step = spinSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * step, Space.World); // หมุนรอบแกนโลก (แนวนอน)
            angleSpun += step;
            yield return null;
        }

        // --- STEP 3: กลับสู่ท่าเดิม (Reset) ---
        yield return new WaitForSeconds(0.5f); // พักแป๊บนึงก่อนดึงกลับ
        yield return StartCoroutine(MoveRotation(_startRot, tiltSpeed / 2));

        yield return new WaitForSeconds(cooldown);
        _ready = true;
    }

    // ฟังก์ชันช่วยหมุนให้สมูท
    IEnumerator MoveRotation(Quaternion target, float speed)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = target;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("โดนกวาดเต็มๆ!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}