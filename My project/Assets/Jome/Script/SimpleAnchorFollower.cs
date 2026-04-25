using UnityEngine;

public class SimpleAnchorFollower : MonoBehaviour
{
    [Header("เป้าหมายที่จะวิ่งตาม")]
    public Transform animatedBody; // ตัวละครเดินปกติ
    public Transform ragdollHips;  // กระดูกสะโพก Ragdoll

    void LateUpdate()
    {
        // เลือกเป้าหมาย: ถ้าตัวเดินเปิดอยู่ให้ไปหาตัวเดิน ถ้าปิดอยู่ให้ไปหาสะโพก Ragdoll
        Transform target = (animatedBody != null && animatedBody.gameObject.activeInHierarchy) 
            ? animatedBody : ragdollHips;

        if (target != null)
        {
            // วาร์ปตำแหน่ง Object นี้ไปที่ตัวละครทันที
            // มันแค่ "ย้ายตำแหน่งตาม" แต่ "ไม่ได้ลากใครไปด้วย" ปัญหาเดินไวเลยหายไปครับ
            transform.position = target.position;
        }
    }
}