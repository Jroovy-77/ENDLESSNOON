using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // ลากส่วนหัว (Head) ของ Ragdoll มาใส่

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 3f;
    public float maxDistance = 8f;

    [Header("Height Offset")]
    public float height = 1f;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minY = -20f;
    public float maxY = 60f;

    [Header("Smooth")]
    public float positionSmooth = 10f;
    public float rotationSmooth = 20f; // ปรับให้ไวขึ้นแบบเกม FPS

    float rotX;
    float rotY;

    void Start()
    {
        // เริ่มต้นด้วยมุมกล้องปัจจุบัน
        Vector3 angles = transform.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 🎮 1. รับค่าจากเมาส์ (หมุนอิสระ ไม่สนท่าทางตัวละคร)
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        // 💡 หัวใจสำคัญ: สร้างการหมุนใหม่โดยใช้ค่าจากเมาส์เพียวๆ 
        // ไม่มีการคูณ target.rotation เข้าไปเด็ดขาด!
        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);

        // 🎯 2. จุดโฟกัส (เอาแค่ตำแหน่งของหัว)
        Vector3 targetPos = target.position + (Vector3.up * height);

        // 🎥 3. คำนวณตำแหน่งที่กล้องควรอยู่ (ห่างจากตัวละครตามทิศทางหมุน)
        Vector3 desiredPos = targetPos - (rotation * Vector3.forward * distance);

        // 🧊 4. ขยับกล้อง (Smooth Position)
        transform.position = Vector3.Lerp(transform.position, desiredPos, positionSmooth * Time.deltaTime);
        
        // 5. ให้กล้องหันไปมองจุดโฟกัสตลอดเวลา
        transform.LookAt(targetPos);
    }
}