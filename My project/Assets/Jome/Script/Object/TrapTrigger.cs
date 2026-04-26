using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [Header("Settings")]
    public StoneLauncher launcher; 
    public string targetTag = "Player"; // ตั้งชื่อ Tag ที่นี่

    private void OnTriggerEnter(Collider other)
    {
        // เช็คดื้อๆ เลยว่าสิ่งที่เดินเข้ามาชน มี Tag ตรงกับที่ตั้งไว้ไหม
        if (other.CompareTag(targetTag))
        {
            if (launcher != null)
            {
                launcher.LaunchStone();
                Debug.Log("Tag ตรงกัน! ยิงหินออกไปแล้ว");
            }
        }
    }
}