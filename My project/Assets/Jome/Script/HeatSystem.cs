using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HeatSystem : MonoBehaviour
{
    [Header("Targets")]
    public GameObject animatedModel; 
    public GameObject ragdollModel;  
    public Transform ragdollHips;    

    [Header("Visual Settings")]
    public Renderer[] characterRenderers; 
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

    [Header("Restart Settings")]
    // เพิ่มตัวแปรนี้เพื่อปรับเวลาการเกิดใหม่ (หน่วยเป็นวินาที)
    [Tooltip("ระยะเวลาที่รอ (วินาที) หลังจากตายก่อนจะเริ่มเกมใหม่")]
    public float restartDelay = 3f; 

    private bool isDead = false;

    void Start()
    {
        if (animatedModel != null) animatedModel.SetActive(true);
        if (ragdollModel != null) ragdollModel.SetActive(false);
    }

    void Update()
    {
        if (sunTransform == null || isDead) return;

        Transform currentTarget = animatedModel.activeInHierarchy ? animatedModel.transform : ragdollHips;
        if (currentTarget == null) return;

        Vector3 origin = currentTarget.position + (Vector3.up * heightOffset);
        
        if (ragdollModel.activeInHierarchy) origin = ragdollHips.position;

        bool isInShade = CheckShade(origin);

        if (isInShade)
            currentHeat -= heatDecreaseRate * Time.deltaTime;
        else
            currentHeat += heatIncreaseRate * Time.deltaTime;

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        UpdateColor();

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

        if (animatedModel != null) animatedModel.SetActive(false);
        if (ragdollModel != null)
        {
            ragdollModel.SetActive(true);
            ragdollModel.transform.position = animatedModel.transform.position;
            ragdollModel.transform.rotation = animatedModel.transform.rotation;
        }

        // ใช้ตัวแปร restartDelay แทนค่าคงที่ 3f
        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}