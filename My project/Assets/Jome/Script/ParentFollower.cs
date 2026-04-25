using UnityEngine;

public class ParentFollower : MonoBehaviour
{
    public Transform animatedBody; // ตัวเดิน
    public Transform ragdollHips;  // สะโพกตัวย้วย

    void LateUpdate()
    {
        // 1. เลือกเป้าหมาย
        Transform target = (animatedBody != null && animatedBody.gameObject.activeInHierarchy) 
            ? animatedBody : ragdollHips;

        if (target != null)
        {
            // 2. ล็อคตำแหน่งตัวพ่อให้เท่ากับตัวละครเป้าหมาย
            // ใช้คำสั่งนี้เพื่อป้องกันปัญหา "เดินไวขึ้น"
            transform.position = target.position;
        }
    }
}