using UnityEngine;
using System.Collections;

public class TrapFall : MonoBehaviour
{
    public enum State { Idle, Falling, Hold }
    public State currentState = State.Idle;

    [Header("Box Detection")]
    public float detectionWidth  = 2f;
    public float detectionLength = 5f;
    public float detectionHeight = 3f;
    public float detectionOffset = 0.5f;

    [Header("Fall Settings")]
    public float fallSpeed = 90f;  // องศา/วินาที
    public float fallAngle = 90f;  // ล้มกี่องศา

    [Header("Timing")]
    public float holdTime = 1.5f;

    // ── Private ──────────────────────────────────────
    private float _currentAngle = 0f;   // มุมปัจจุบัน
    private Vector3 _pivotPoint;        // จุดหมุนที่ฐาน
    private Vector3 _pivotAxis;         // แกนหมุน

    void Start()
    {
        // จุดหมุนอยู่ที่ฐานของ Object (ลบลงครึ่งนึงของ Collider)
        Collider col = GetComponent<Collider>();
        float halfHeight = col != null ? col.bounds.extents.y : 1f;

        _pivotPoint = transform.position - transform.up * halfHeight;
        _pivotAxis  = transform.right; // หมุนรอบแกนขวา = ล้มไปข้างหน้า
    }

    void Update()
    {
        if (currentState != State.Idle) return;
        if (DetectPlayer()) StartCoroutine(FallRoutine());
    }

    // ══════════════════════════════════════════════════
    //  DETECTION — Box ด้านหน้า
    // ══════════════════════════════════════════════════
    bool DetectPlayer()
    {
        Vector3 center = transform.position
                       + transform.forward * (detectionOffset + detectionLength * 0.5f)
                       + Vector3.up        * (detectionHeight * 0.5f);

        Vector3 halfSize = new Vector3(
            detectionWidth  * 0.5f,
            detectionHeight * 0.5f,
            detectionLength * 0.5f);

        Collider[] hits = Physics.OverlapBox(center, halfSize, transform.rotation);
        foreach (var hit in hits)
            if (hit.CompareTag("Player")) return true;

        return false;
    }

    // ══════════════════════════════════════════════════
    //  MAIN ROUTINE
    // ══════════════════════════════════════════════════
    IEnumerator FallRoutine()
    {
        currentState = State.Falling;
        yield return StartCoroutine(Fall());

        currentState = State.Hold;
        yield return new WaitForSeconds(holdTime);

        // พังทิ้งไว้ ไม่ลุกกลับ
        // ถ้าอยากให้ Reset เรียก ResetTrap() จากภายนอก
    }

    // ══════════════════════════════════════════════════
    //  FALL — หมุนรอบฐานโดยไม่ใช้ Pivot
    // ══════════════════════════════════════════════════
    IEnumerator Fall()
    {
        while (_currentAngle < fallAngle)
        {
            float step = fallSpeed * Time.deltaTime;
            step = Mathf.Min(step, fallAngle - _currentAngle); // ไม่เกินมุมสูงสุด

            // หมุน Object รอบจุดฐาน
            transform.RotateAround(_pivotPoint, _pivotAxis, step);
            _currentAngle += step;

            yield return null;
        }
    }

    // ══════════════════════════════════════════════════
    //  RESET — เรียกจากภายนอกเมื่อ Player ตาย
    // ══════════════════════════════════════════════════
    public void ResetTrap()
    {
        StopAllCoroutines();

        // หมุนกลับ
        transform.RotateAround(_pivotPoint, _pivotAxis, -_currentAngle);
        _currentAngle = 0f;
        currentState  = State.Idle;
    }

    // ══════════════════════════════════════════════════
    //  DAMAGE
    // ══════════════════════════════════════════════════
    void OnCollisionEnter(Collision col)
    {
        if (currentState != State.Falling) return;
        if (!col.gameObject.CompareTag("Player")) return;

        // TODO: ดาเมจ
        // col.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(50);
        Debug.Log("ล้มทับ Player!");
    }

    // ══════════════════════════════════════════════════
    //  GIZMOS
    // ══════════════════════════════════════════════════
    void OnDrawGizmosSelected()
    {
        // กล่องตรวจจับ
        Vector3 center = transform.position
                       + transform.forward * (detectionOffset + detectionLength * 0.5f)
                       + Vector3.up        * (detectionHeight * 0.5f);

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawCube(Vector3.zero,
            new Vector3(detectionWidth, detectionHeight, detectionLength));

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireCube(Vector3.zero,
            new Vector3(detectionWidth, detectionHeight, detectionLength));

        Gizmos.matrix = old;

        // ทิศล้ม
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * (detectionOffset + detectionLength));
    }
}