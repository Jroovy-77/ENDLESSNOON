using UnityEngine;

public class PlayerScoreSubmit : MonoBehaviour
{
    // นับจำนวนครั้งที่ตายจาก PlayerPrefs (ข้ามรอบได้)
    void OnDestroy()
    {
        if (!Application.isPlaying) return;

        // +1 ทุกครั้งที่ตาย
        int deaths = PlayerPrefs.GetInt("DeathCount", 0) + 1;
        PlayerPrefs.SetInt("DeathCount", deaths);
        PlayerPrefs.Save();

        string username = PlayerPrefs.GetString("Username", "Player");

        LootLockerManager.Instance?.SubmitScore(
            username:  username,
            score:     deaths,
            onSuccess: () => Debug.Log("ส่งคะแนนสำเร็จ ตายแล้ว: " + deaths + " ครั้ง"),
            onFail:    (err) => Debug.LogWarning("ส่งไม่สำเร็จ: " + err)
        );
    }

    // เรียกตอนเริ่มเกมใหม่ถ้าอยากรีเซ็ต
    public static void ResetDeaths()
    {
        PlayerPrefs.SetInt("DeathCount", 0);
        PlayerPrefs.Save();
    }
}