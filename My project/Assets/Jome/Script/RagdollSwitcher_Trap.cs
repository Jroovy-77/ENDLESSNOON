using UnityEngine;

public class RagdollResurrector : MonoBehaviour
{
    [Header("Body References")]
    public GameObject animatedBody; // ตัวละครปกติ (ที่มี Animator)
    public GameObject ragdollBody;  // ตัวละคร Ragdoll (ตัวย้วย)
    public Transform ragdollHips;   // ลากกระดูก "Hips" หรือ "Pelvis" ของตัว Ragdoll มาใส่

    [Header("Settings")]
    public string trapTag = "Trap";   // ชื่อ Tag ของวัตถุที่ชนแล้วล้ม
    public float knockbackForce = 20f; // แรงกระเด็น
    public float recoverTime = 3f;    // เวลาก่อนจะลุกขึ้นมาใหม่ (วินาที)

    private bool isRagdoll = false;

    void Start()
    {
        // เริ่มเกม: เปิดตัวเดิน ปิดตัวย้วย
        animatedBody.SetActive(true);
        ragdollBody.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        // ถ้าโดนชนด้วย Tag ที่กำหนด และยังไม่ล้ม
        if (collision.gameObject.CompareTag(trapTag) && !isRagdoll)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            ActivateRagdoll(hitPoint);
        }
    }

    public void ActivateRagdoll(Vector3 hitPoint)
    {
        isRagdoll = true;

        // 1. จำตำแหน่งล่าสุดก่อนสลับ
        Vector3 currentPos = animatedBody.transform.position;
        Quaternion currentRot = animatedBody.transform.rotation;

        // 2. สลับตัว
        animatedBody.SetActive(false);
        ragdollBody.SetActive(true);

        // 3. ย้าย Ragdoll ไปที่จุดเกิดเหตุ
        ragdollBody.transform.position = currentPos;
        ragdollBody.transform.rotation = currentRot;

        // 4. ใส่แรงกระแทกให้กระดูกทุกชิ้นกระเด็นกระจาย
        Rigidbody[] rbs = ragdollBody.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.AddExplosionForce(knockbackForce, hitPoint, 5f, 2f, ForceMode.Impulse);
        }

        // 5. นับถอยหลังเพื่อลุกขึ้นมาใหม่
        Invoke("Recover", recoverTime);
    }

    void Recover()
    {
        // 🔥 จุดสำคัญ: ก่อนจะลุก ต้องย้าย "ตัวพ่อ" ไปหา "สะโพก" ที่นอนกองอยู่บนพื้น
        if (ragdollHips != null)
        {
            // ย้ายตำแหน่งตัวพ่อไปที่ตำแหน่งสะโพก (ตัดแกน Y ให้เท้าติดพื้นพอดี)
            Vector3 spawnPos = ragdollHips.position;
            // spawnPos.y += 0.1f; // ยกขึ้นนิดหน่อยกันจมดิน
            
            transform.position = spawnPos;
        }

        // สลับตัวกลับ
        animatedBody.SetActive(true);
        ragdollBody.SetActive(false);
        
        isRagdoll = false;
        Debug.Log("ลุกขึ้นมาแล้ว!");
    }
}