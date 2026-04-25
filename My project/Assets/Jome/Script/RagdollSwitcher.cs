using UnityEngine;

public class RagdollSwitcher_Trap : MonoBehaviour
{
    // เราต้องหาตัว Ragdoll เพื่อเปิดมันตอนโดนชน
    public GameObject ragdollBody; 
    
    // ไว้เช็คว่าตัวย้วยแล้วหรือยัง ป้องกันโดนชนซ้ำแล้ว Error
    private bool isRagdoll = false;

    void Start()
    {
        // เริ่มเกมมา เช็คว่าได้ลากตัว Ragdoll ใส่ช่องหรือยัง
        if (ragdollBody == null)
        {
            Debug.LogError("เฮ้ย! นายลืมลากตัว RagdollBody ใส่ในช่องสคริปต์บน AnimatedBody หรือเปล่า?");
        }
    }

    // เมื่อตัวเดินปกติชนกับอะไรบางอย่าง
    void OnCollisionEnter(Collision collision)
    {
        // ถ้าวัตถุที่ชนมี Tag ว่า "Trap" และเรายังไม่ได้เป็น Ragdoll
        if (collision.gameObject.CompareTag("Trap") && !isRagdoll)
        {
            ActivateRagdoll();
        }
    }

    public void ActivateRagdoll()
    {
        // ป้องกัน Error ถ้าไม่ได้ใส่ตัว Ragdoll ไว้
        if (ragdollBody == null) return;

        isRagdoll = true;

        // 1. เก็บพิกัดและองศาของตัวเดินปกติไว้ (เพื่อใช้ส่งต่อ)
        Vector3 lastPos = transform.position;
        Quaternion lastRot = transform.rotation;

        // 2. เปิดตัว Ragdoll ขึ้นมาแทนที่ทันที
        ragdollBody.SetActive(true);

        // 3. ย้ายตัว Ragdoll ไปที่จุดที่โดนชนทันที
        ragdollBody.transform.position = lastPos;
        ragdollBody.transform.rotation = lastRot;

        // 4. (Optional) ใส่แรงถีบที่ "สะโพก" ของ Ragdoll ให้มันกระเด็น
        // หา Rigidbody ที่เป็นตัวลูกชิ้นแรก (ส่วนใหญ่จะเป็นสะโพก)
        Rigidbody hipRb = ragdollBody.GetComponentInChildren<Rigidbody>();
        if (hipRb != null)
        {
            // ใส่แรงกระแทกขึ้นข้างบนและไปทางหลังเล็กน้อย
            hipRb.AddForce(Vector3.up * 8f + Vector3.back * 5f, ForceMode.Impulse);
        }

        // 5. ปิดตัวเดินปกติทิ้งไป (ทำให้ดูเหมือนมันสลับตัวกันเป๊ะ)
        this.gameObject.SetActive(false);

        // หมายเหตุ: สคริปต์นี้จะทำให้ย้วยแล้วย้วยเลย
        // ถ้าอยากให้ฟื้นตัว ต้องเขียนโค้ดเพิ่มใน RagdollBody เพื่อให้มัน Active ตัวเดินปกติกลับมาครับ
    }
}