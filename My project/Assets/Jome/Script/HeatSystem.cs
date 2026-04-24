using UnityEngine;
using UnityEngine.SceneManagement;

public class HeatSystem : MonoBehaviour
{
    [Header("Heat Settings")]
    public float maxHeat = 100f;
    public float currentHeat = 0f;

    public float heatIncreaseRate = 10f;
    public float heatDecreaseRate = 20f;

    [Header("Sun Settings")]
    public Transform sunTransform; // ลาก Directional Light มาใส่

    [Header("Raycast Settings")]
    public float rayDistance = 30f;
    public LayerMask obstacleLayer;

    [Header("Debug")]
    public bool showDebugRay = true;

    void Update()
    {
        bool isInShade = CheckShade();

        if (isInShade)
            currentHeat -= heatDecreaseRate * Time.deltaTime;
        else
            currentHeat += heatIncreaseRate * Time.deltaTime;

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        if (currentHeat >= maxHeat)
        {
            Die();
        }
    }

    bool CheckShade()
    {
        if (sunTransform == null)
        {
            Debug.LogError("❌ ยังไม่ได้ใส่ Directional Light!");
            return false;
        }

        Vector3 sunDir = -sunTransform.forward;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Vector3[] offsets = {
            Vector3.zero,
            Vector3.right * 0.3f,
            Vector3.left * 0.3f
        };

        foreach (var offset in offsets)
        {
            Vector3 start = origin + offset;

            if (showDebugRay)
                Debug.DrawRay(start, sunDir * rayDistance, Color.red);

            if (!Physics.Raycast(start, sunDir, rayDistance, obstacleLayer))
            {
                return false; // โดนแดด
            }
        }

        return true; // อยู่ในเงา
    }

    void Die()
    {
        Debug.Log("🔥 Player Burned!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}