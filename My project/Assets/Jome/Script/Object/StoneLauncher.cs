using UnityEngine;

public class StoneLauncher : MonoBehaviour
{
    [Header("Detection Box Settings")]
    public Vector3 boxSize = new Vector3(5f, 2f, 5f); // ขนาดของกล่องสี่เหลี่ยม (กว้าง, สูง, ลึก)
    public Vector3 boxOffset;                         // ตำแหน่งเลื่อนของกล่องจากตัวเครื่องยิง
    public LayerMask playerLayer;                     // เลือกเลเยอร์ Player เพื่อความแม่นยำ

    [Header("References")]
    public GameObject stonePrefab;  
    public Transform shootPoint;    

    [Header("Settings")]
    public float launchForce = 30f; 
    public float fireRate = 0.5f;   
    private float nextFireTime;

    void Update()
    {
        // ตรวจสอบว่ามี Player อยู่ในกล่องสี่เหลี่ยมที่กำหนดหรือไม่
        if (IsPlayerInBox())
        {
            LaunchStone();
        }
    }

    bool IsPlayerInBox()
    {
        // สร้างกล่องทิพย์ขึ้นมาเช็คการชน
        Collider[] hitColliders = Physics.OverlapBox(transform.position + boxOffset, boxSize / 2, transform.rotation, playerLayer);
        
        // ถ้าในกล่องมี Object อย่างน้อย 1 ตัว แปลว่าผู้เล่นเข้ามาแล้ว
        return hitColliders.Length > 0;
    }

    public void LaunchStone()
    {
        if (Time.time < nextFireTime) return;

        if (stonePrefab != null && shootPoint != null)
        {
            GameObject stone = Instantiate(stonePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody rb = stone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(shootPoint.forward * launchForce, ForceMode.Impulse);
            }

            nextFireTime = Time.time + fireRate;
            Debug.Log("🪨 ตรวจพบในระยะสี่เหลี่ยม! ยิง!!");
        }
    }

    // วาดรูปทรงสี่เหลี่ยมในหน้า Scene เพื่อให้ตั้งค่าง่าย
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero + boxOffset, boxSize);
    }
}