using UnityEngine;
using LootLocker.Requests;
using System;

public class LootLockerManager : MonoBehaviour
{
    public static LootLockerManager Instance { get; private set; }

    [Header("LootLocker Settings")]
    public string leaderboardKey = "YOUR_LEADERBOARD_KEY";

    private bool _isReady = false;
    public  bool IsReady  => _isReady;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => Login();

    // ── Login ────────────────────────────────────────
    public void Login(Action onSuccess = null, Action<string> onFail = null)
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                _isReady = true;

                // บันทึกชื่อผู้เล่นเข้า LootLocker ด้วย
                string username = PlayerPrefs.GetString("Username", "Player");
                SetPlayerName(username);

                onSuccess?.Invoke();
            }
            else
            {
                onFail?.Invoke(response.errorData?.message ?? "Login failed");
            }
        });
    }

    // ── Set Player Name ──────────────────────────────
    void SetPlayerName(string username)
    {
        LootLockerSDKManager.SetPlayerName(username, (response) =>
        {
            if (response.success)
                Debug.Log("ตั้งชื่อสำเร็จ: " + username);
            else
                Debug.LogWarning("ตั้งชื่อไม่สำเร็จ: " + response.errorData);
        });
    }

    // ── Submit Score ─────────────────────────────────
    public void SubmitScore(string username, int score,
                            Action onSuccess = null, Action<string> onFail = null)
    {
        if (!_isReady) { onFail?.Invoke("ยังไม่ได้ Login"); return; }

        LootLockerSDKManager.SubmitScore(username, score, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                onSuccess?.Invoke();
                Debug.Log("ส่งคะแนนสำเร็จ: " + score);
            }
            else
                onFail?.Invoke(response.errorData?.message ?? "Submit failed");
        });
    }

    // ── Get Leaderboard ──────────────────────────────
    public void GetLeaderboard(Action<LootLockerLeaderboardMember[]> onSuccess,
                               Action<string> onFail = null)
    {
        if (!_isReady) { onFail?.Invoke("ยังไม่ได้ Login"); return; }

        LootLockerSDKManager.GetScoreList(leaderboardKey, 100, 0, (response) =>
        {
            if (response.success)
                onSuccess?.Invoke(response.items);
            else
                onFail?.Invoke(response.errorData?.message ?? "Load failed");
        });
    }
}