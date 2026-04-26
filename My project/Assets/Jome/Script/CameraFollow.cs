using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    public Transform animatedBody; 
    public Transform ragdollHips;  

    [Header("Distance & Collision")]
    public float distance = 5f;
    public float minDistance = 1f;
    public float maxDistance = 8f;
    public float cameraRadius = 0.2f; // รัศมีของกล้องเพื่อเช็คการชน
    public LayerMask collisionLayers; // เลือก Layer ที่จะให้กล้องชน (เช่น Ground, Default)

    [Header("Height Offset")]
    public float height = 1f;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minY = -20f;
    public float maxY = 60f;

    [Header("Smooth")]
    public float positionSmooth = 15f; // เพิ่มความเร็วในการตามเพื่อลดอาการดีเลย์
    public float rotationSmooth = 10f;

    float rotX;
    float rotY;
    float currentDistance;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;
        currentDistance = distance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        Transform currentTarget = (animatedBody != null && animatedBody.gameObject.activeInHierarchy) 
            ? animatedBody : ragdollHips;

        if (currentTarget == null) return;

        // 🎮 1. รับค่าจากเมาส์
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        // คำนวณการหมุน (ใช้ Slerp เพื่อความนุ่มนวล)
        Quaternion targetRotation = Quaternion.Euler(rotY, rotX, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth * Time.deltaTime);

        // 🎯 2. จุดโฟกัส
        Vector3 targetPos = currentTarget.position + (Vector3.up * height);

        // 🎥 3. ระบบกันกล้องทะลุกำแพง (Camera Collision)
        currentDistance = CheckCameraCollision(targetPos, targetRotation);

        // 🎥 4. คำนวณตำแหน่งที่กล้องควรอยู่
        Vector3 desiredPos = targetPos - (targetRotation * Vector3.forward * currentDistance);

        // 🧊 5. ขยับกล้อง (ใช้ Lerp แบบความเร็วสูงเพื่อลดอาการสั่น)
        transform.position = Vector3.Lerp(transform.position, desiredPos, positionSmooth * Time.deltaTime);
        
        // 6. มองเป้าหมายเสมอ
        transform.LookAt(targetPos);
    }

    // ฟังก์ชันเช็คการชนของกล้อง
    float CheckCameraCollision(Vector3 focusPoint, Quaternion rotation)
    {
        float targetDist = distance;
        Vector3 dir = rotation * Vector3.back; // ทิศทางที่กล้องถอยหลัง

        RaycastHit hit;
        // ยิงลำแสงจากจุดโฟกัสถอยหลังไปหาตำแหน่งกล้อง
        if (Physics.SphereCast(focusPoint, cameraRadius, dir, out hit, distance, collisionLayers))
        {
            // ถ้าชนอะไร ให้กล้องขยับมาอยู่ที่จุดที่ชน (ลบออกนิดหน่อยกันจม)
            return Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        return distance;
    }
}