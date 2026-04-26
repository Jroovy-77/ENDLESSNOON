using UnityEngine;
using UnityEngine.SceneManagement;

public class HeatSystem : MonoBehaviour
{
    [Header("Targets (ใส่ให้ครบ)")]
    public Transform animatedBody; // ตัวเดินปกติ
    public Transform ragdollHips;  // สะโพกตัวย้วย

    [Header("Heat Settings")]
    public float maxHeat = 100f;
    public float currentHeat = 0f;
    public float heatIncreaseRate = 15f;
    public float heatDecreaseRate = 25f;

    [Header("Sun Settings")]
    public Transform sunTransform;

    [Header("Raycast Config")]
    public float rayDistance = 100f;
    public LayerMask obstacleLayer;

    // 🔥 เพิ่มช่องสำหรับกำหนดความสูง
    [Header("Raycast Position Adjust")]
    public float heightOffset = 0.5f; // ค่าเริ่มต้นคือ 0.5 เมตร (ลองปรับดูใน Unity)

    void Update()
    {
        if (sunTransform == null) return;

        Transform currentTarget = (animatedBody != null && animatedBody.gameObject.activeInHierarchy) 
            ? animatedBody : ragdollHips;

        if (currentTarget == null) return;

        // 💡 เปลี่ยนตรงนี้: เอารากของตำแหน่งมาบวกกับความสูงที่เราตั้งไว้
        Vector3 origin = currentTarget.position + (Vector3.up * heightOffset);
        
        // ถ้าเป็น Ragdoll อาจจะต้องใช้ Hips ยิงแสงเหมือนเดิม เพราะมันนอนกองอยู่
        if (ragdollHips != null && ragdollHips.gameObject.activeInHierarchy)
        {
            origin = ragdollHips.position; // ใช้ตำแหน่ง Hips ตรงๆ ตอนย้วย
        }

        bool isInShade = CheckShade(origin);

        // ... ส่วนคำนวณ Heat และ Die() เหมือนเดิม ...
        if (isInShade)
            currentHeat -= heatDecreaseRate * Time.deltaTime;
        else
            currentHeat += heatIncreaseRate * Time.deltaTime;

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);
        if (currentHeat >= maxHeat) Die();
    }

    bool CheckShade(Vector3 checkPos)
    {
        Vector3 sunDirection = -sunTransform.forward;
        Vector3 startPoint = checkPos + (sunDirection * 0.2f);

        if (Physics.Raycast(startPoint, sunDirection, out RaycastHit hit, rayDistance, obstacleLayer))
        {
            Debug.DrawLine(startPoint, hit.point, Color.green);
            return true; 
        }

        Debug.DrawRay(startPoint, sunDirection * rayDistance, Color.red);
        return false;
    }

    void Die()
    {
        currentHeat = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}