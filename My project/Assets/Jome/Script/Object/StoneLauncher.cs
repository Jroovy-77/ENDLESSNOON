using UnityEngine;

public class StoneLauncher : MonoBehaviour
{
    [Header("References")]
    public GameObject stonePrefab;  // ลาก Prefab ก้อนหินมาใส่ (ก้อนหินต้องติด Tag = Trap)
    public Transform shootPoint;    // จุดที่หินจะโผล่ออกมา (สร้าง Empty Object เป็นจุดยิง)

    [Header("Settings")]
    public float launchForce = 30f; // แรงยิง
    public float fireRate = 0.5f;   // ระยะห่างต่อการยิง 1 นัด
    private float nextFireTime;

    public void LaunchStone()
    {
        if (Time.time < nextFireTime) return;

        // 1. สร้างก้อนหิน
        GameObject stone = Instantiate(stonePrefab, shootPoint.position, shootPoint.rotation);
        
        // 2. ดีดหินออกไป (ทิศทางไปข้างหน้าของจุดยิง)
        Rigidbody rb = stone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootPoint.forward * launchForce, ForceMode.Impulse);
        }

        nextFireTime = Time.time + fireRate;
        Debug.Log("ยิงหินใส่แล้ว!");
    }
}