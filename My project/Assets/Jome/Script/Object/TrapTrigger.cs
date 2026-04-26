using UnityEngine;
using UnityEngine.Events; // ต้องมีตัวนี้เพิ่มมา

public class TrapTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Player";

    [Header("Actions (กด + ใน Inspector เพื่อเพิ่มสิ่งที่อยากให้เกิด)")]
    public UnityEvent onTriggerActivated; 

    private void OnTriggerEnter(Collider other)
    {
        // เช็คว่าใช่คนเหยียบไหม
        if (other.CompareTag(targetTag))
        {
            // สั่งให้ทุกอย่างที่อยู่ในลิสต์ทำงาน!
            if (onTriggerActivated != null)
            {
                onTriggerActivated.Invoke();
                Debug.Log("Trigger Activated! สั่งงานทุกอย่างในรายการแล้ว");
            }
        }
    }
}