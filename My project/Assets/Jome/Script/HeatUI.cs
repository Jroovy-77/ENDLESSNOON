using UnityEngine;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    public HeatSystem heatSystem; // ลาก PlayerParent ที่มี HeatSystem มาใส่
    public Image fillImage;

    [Header("Colors")]
    public Color coolColor = new Color(0.2f, 0.6f, 1f);
    public Color hotColor = new Color(1f, 0.2f, 0.1f);

    void Start()
    {
        // ถ้าลืมลากใส่ ให้ลองหาในฉากเองอัตโนมัติจาก Tag "Player"
        if (heatSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                heatSystem = player.GetComponent<HeatSystem>();
            }
        }
    }

    void Update()
    {
        if (heatSystem == null) return;

        // ป้องกันการหารด้วยศูนย์
        float maxHeat = heatSystem.maxHeat > 0 ? heatSystem.maxHeat : 100f;
        float percent = heatSystem.currentHeat / maxHeat;

        // Fill bar
        fillImage.fillAmount = percent;

        // Gradient color
        fillImage.color = Color.Lerp(coolColor, hotColor, percent);

        // Pulse effect เมื่อความร้อนสูง (เกิน 75%)
        if (percent > 0.75f)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 12f) * 0.08f;
            fillImage.rectTransform.localScale = new Vector3(pulse, pulse, 1f); // ปรับให้ขยายทั้ง X และ Y จะดูสวยกว่า
        }
        else
        {
            fillImage.rectTransform.localScale = Vector3.one;
        }
    }
}