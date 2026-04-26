using UnityEngine;

public class RotateWhenPlayerNear : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // ลาก Player มาใส่

    [Header("Range")]
    public float detectRange = 5f; // ระยะตรวจจับ

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // แกนหมุน
    public float rotationSpeed = 100f;        // ความเร็วหมุน

    private bool isRotating = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // เข้าใกล้ → เริ่มหมุน
        if (distance <= detectRange)
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

        if (isRotating)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }

    // วาดเส้นระยะใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}