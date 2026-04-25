using UnityEngine;

public class RagdollResurrector_Final : MonoBehaviour
{
    public GameObject animatedBody; 
    public GameObject ragdollBody;  
    public Transform ragdollHips;   

    void Update()
    {
        // 💡 ถ้าตัวเดินปกติทำงานอยู่ ให้ตัวพ่อล็อคตำแหน่งตามลูก "แค่ตำแหน่ง" 
        // โดยที่ตัวลูกต้องไม่มีสคริปต์ขยับตัวเอง (หรือขยับแบบ Local)
        // แต่ทางที่ดีที่สุดคือ: ให้สคริปต์เดินขยับที่ "ตัวพ่อ" แทนครับ
        
        if (animatedBody.activeInHierarchy)
        {
            // ล็อคให้ Parent อยู่ที่เดียวกับตัวเดิน เพื่อให้กล้องตามทัน
            // แต่ถ้าเดินไวไป ให้ปิดบรรทัดนี้แล้วย้ายสคริปต์เดินมาไว้ที่ Parent แทน
            transform.position = animatedBody.transform.position;
        }
    }

    public void ActivateRagdoll() 
    {
        animatedBody.SetActive(false);
        ragdollBody.SetActive(true);
        // ตอนย้วยไม่ต้องตาม ปล่อยให้มันกลิ้งไป
    }

    public void Recover()
    {
        // ย้ายตัวพ่อไปหาจุดที่สะโพกนอนอยู่ "ครั้งเดียว" ตอนจะลุก
        if (ragdollHips != null)
        {
            transform.position = ragdollHips.position;
        }
        
        animatedBody.SetActive(true);
        ragdollBody.SetActive(false);
    }
}