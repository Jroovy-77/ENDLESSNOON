using UnityEngine;
using UnityEngine.SceneManagement;

public class HeatSystem : MonoBehaviour
{
    [Header("Target (สำคัญมาก)")]
    public Transform playerTarget;    // ลากกระดูก Head หรือกระดูกส่วนที่ต้องการให้เส้นตามมาใส่

    [Header("Heat Settings")]
    public float maxHeat = 100f;
    public float currentHeat = 0f;
    public float heatIncreaseRate = 15f;
    public float heatDecreaseRate = 25f;

    [Header("Sun Settings")]
    public Transform sunTransform;

    [Header("Raycast Config")]
    public float rayDistance = 100f;
    public LayerMask obstacleLayer;  // อย่าลืมเอา Layer Player ออกจากช่องนี้

    void Update()
    {
        if (sunTransform == null) return;

        // 💡 ถ้าเราลากเป้าหมายมาใส่แล้ว ให้ใช้ตำแหน่งของเป้าหมายนั้นโดยตรง!
        // ถ้าไม่ลากมาค่อยใช้ตำแหน่งของตัวเอง (Fallback)
        Vector3 origin = (playerTarget != null) ? playerTarget.position : transform.position;

        bool isInShade = CheckShade(origin);

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
        
        // ขยับจุดยิงให้ลอยเหนือเป้าหมายนิดหน่อยกันการชนตัวเอง
        Vector3 startPoint = checkPos + (sunDirection * 0.2f);

        if (Physics.Raycast(startPoint, sunDirection, out RaycastHit hit, rayDistance, obstacleLayer))
        {
            // วาดเส้นสีเขียวจากจุดที่มันตาม (Target) ไปหาวัตถุที่ชน
            Debug.DrawLine(startPoint, hit.point, Color.green);
            return true; 
        }

        // วาดเส้นสีแดงพุ่งขึ้นฟ้าจากจุดที่มันตาม (Target)
        Debug.DrawRay(startPoint, sunDirection * rayDistance, Color.red);
        return false;
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}