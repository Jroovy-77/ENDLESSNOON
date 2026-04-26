using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets (ใส่ให้ครบทั้ง 2 ตัว)")]
    public Transform animatedBody; // ลากตัวเดินปกติมาใส่
    public Transform ragdollHips;  // ลากสะโพก Ragdoll มาใส่

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

    float rotX;
    float rotY;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // 🔥 เลือกเป้าหมายอัตโนมัติ: ถ้าตัวเดินเปิดอยู่ให้ตามตัวเดิน ถ้าปิดอยู่ให้ตาม Ragdoll
        Transform currentTarget = (animatedBody != null && animatedBody.gameObject.activeInHierarchy) 
            ? animatedBody : ragdollHips;

        if (currentTarget == null) return;

        // 🎮 1. รับค่าจากเมาส์
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);

        // 🎯 2. จุดโฟกัส
        Vector3 targetPos = currentTarget.position + (Vector3.up * height);

        // 🎥 3. คำนวณตำแหน่งที่กล้องควรอยู่
        Vector3 desiredPos = targetPos - (rotation * Vector3.forward * distance);

        // 🧊 4. ขยับกล้อง
        transform.position = Vector3.Lerp(transform.position, desiredPos, positionSmooth * Time.deltaTime);
        
        // 5. มองเป้าหมาย
        transform.LookAt(targetPos);
    }
}