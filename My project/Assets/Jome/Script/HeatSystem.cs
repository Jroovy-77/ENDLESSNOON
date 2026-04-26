using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ต้องมีตัวนี้เพื่อใช้หน่วงเวลา (Coroutine)

public class HeatSystem : MonoBehaviour
{
    [Header("Targets")]
    public GameObject animatedModel; // ตัวละครร่างเดิน (ที่มี Animator)
    public GameObject ragdollModel;  // ตัวละครร่างย้วย (Ragdoll)
    public Transform ragdollHips;    // สะโพกของร่างย้วยไว้เช็คแดดตอนล้ม

    [Header("Visual Settings")]
    public Renderer[] characterRenderers; // ลาก Mesh ทุกชิ้นมาใส่ (รองรับหลายชิ้น)
    public Color normalColor = Color.white;
    public Color burntColor = Color.black;

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
    public float heightOffset = 0.5f;

    private bool isDead = false;

    void Start()
    {
        // เริ่มเกมมา ร่างเดินต้องเปิด ร่างย้วยต้องปิด
        if (animatedModel != null) animatedModel.SetActive(true);
        if (ragdollModel != null) ragdollModel.SetActive(false);
    }

    void Update()
    {
        if (sunTransform == null || isDead) return;

        // เช็คตำแหน่งว่าตอนนี้ใช้ร่างไหนอยู่
        Transform currentTarget = animatedModel.activeInHierarchy ? animatedModel.transform : ragdollHips;
        if (currentTarget == null) return;

        Vector3 origin = currentTarget.position + (Vector3.up * heightOffset);
        
        // ถ้าเป็น Ragdoll ให้ใช้ตำแหน่งสะโพกตรงๆ
        if (ragdollModel.activeInHierarchy) origin = ragdollHips.position;

        bool isInShade = CheckShade(origin);

        // คำนวณความร้อน
        if (isInShade)
            currentHeat -= heatDecreaseRate * Time.deltaTime;
        else
            currentHeat += heatIncreaseRate * Time.deltaTime;

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        // อัปเดตสีตัวละคร (ขาว -> ดำ)
        UpdateColor();

        // ถ้าความร้อนเต็ม = ตาย
        if (currentHeat >= maxHeat)
        {
            StartCoroutine(DieRoutine());
        }
    }

    void UpdateColor()
    {
        float ratio = currentHeat / maxHeat;
        foreach (Renderer r in characterRenderers)
        {
            if (r != null)
            {
                r.material.color = Color.Lerp(normalColor, burntColor, ratio);
            }
        }
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

    IEnumerator DieRoutine()
    {
        isDead = true;
        Debug.Log("ตัวไหม้เกรียมจนสลบ!");

        // 1. สลับร่าง: ปิดร่างเดิน เปิดร่างย้วย
        if (animatedModel != null) animatedModel.SetActive(false);
        if (ragdollModel != null)
        {
            ragdollModel.SetActive(true);
            // ก๊อปปี้ตำแหน่งจากร่างเดินไปร่างย้วยเพื่อให้ล้มตรงจุดเดิม
            ragdollModel.transform.position = animatedModel.transform.position;
            ragdollModel.transform.rotation = animatedModel.transform.rotation;
        }

        // 2. รอ 3 วินาที ให้ผู้เล่นดูร่างที่ไหม้เกรียมล้มย้วย
        yield return new WaitForSeconds(3f);

        // 3. เริ่มเกมใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}