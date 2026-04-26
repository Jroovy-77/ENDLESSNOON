using UnityEngine;
using UnityEngine.Events; // ต้องมีตัวนี้เพื่อใช้ระบบ Event

public class TrapTrigger : MonoBehaviour
{
    [Header("Target Settings")]
    public string targetTag = "Player";
    public bool triggerOnlyOnce = true; // ติ๊กไว้เพื่อให้ทำงานแค่ครั้งเดียว

    [Header("Actions")]
    // ช่องนี้จะเหมือนปุ่ม Button ใน UI ลากอะไรมาใส่ก็ได้
    public UnityEvent onTriggerActivated; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // เช็ค Tag และเช็คว่าเคยทำงานไปหรือยัง
        if (other.CompareTag(targetTag) && !hasTriggered)
        {
            if (onTriggerActivated != null)
            {
                onTriggerActivated.Invoke(); // สั่งงานทุกอย่างในรายการ
                Debug.Log("Trigger Activated!");
                
                if (triggerOnlyOnce) hasTriggered = true;
            }
        }
    }
}