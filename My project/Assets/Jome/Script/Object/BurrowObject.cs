using UnityEngine;

public class BurrowObject : MonoBehaviour
{
    [Header("Settings")]
    public float burrowDepth = 2f;    // มุดลงไปลึกแค่ไหน
    public float moveSpeed = 5f;     // ความเร็วตอนมุด
    public string playerTag = "Player"; // Tag ของตัวละคร

    private Vector3 targetPosition;
    private bool shouldBurrow = false;

    void Start()
    {
        // กำหนดตำแหน่งเป้าหมาย (ตำแหน่งปัจจุบันลบความสูงลงไป)
        targetPosition = transform.position + (Vector3.down * burrowDepth);
    }

    void Update()
    {
        if (shouldBurrow)
        {
            // ค่อยๆ เคลื่อนที่ลงดิน
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            
            // ถ้ามุดจนเกือบถึงเป้าหมายแล้ว ให้ทำลายตัวเองทิ้ง (หรือปิด Object)
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ถ้า Player เดินมาชน (แนะนำให้ใช้ Collider แบบ Is Trigger ครอบไว้รอบๆ)
        if (other.CompareTag(playerTag))
        {
            shouldBurrow = true;
            Debug.Log("ว้าย! มุดหนีดีกว่า");
        }
    }
}