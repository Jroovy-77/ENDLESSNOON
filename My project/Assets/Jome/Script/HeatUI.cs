using UnityEngine;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    public HeatSystem heatSystem;
    public Image fillImage;

    [Header("Colors")]
    public Color coolColor = new Color(0.2f, 0.6f, 1f); // ฟ้า
    public Color hotColor = new Color(1f, 0.2f, 0.1f); // แดง

    void Update()
    {
        float percent = heatSystem.currentHeat / heatSystem.maxHeat;

        // Fill bar
        fillImage.fillAmount = percent;

        // Gradient color
        fillImage.color = Color.Lerp(coolColor, hotColor, percent);

        // Pulse effect ตอนใกล้ตาย
        if (percent > 0.75f)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 12f) * 0.08f;
            fillImage.rectTransform.localScale = new Vector3(pulse, 1f, 1f);
        }
        else
        {
            fillImage.rectTransform.localScale = Vector3.one;
        }
    }
}