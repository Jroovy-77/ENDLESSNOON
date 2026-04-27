using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// GameOverUI — แสดงคะแนน, Submit, View Ranking
/// ติดบน Game Over Panel
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI statusText;
    public Button          submitButton;
    public Button          rankingButton;
    public GameObject      leaderboardPanel;

    private int  _score;
    private bool _submitted = false;

    // ══════════════════════════════════════════════════
    //  เรียกตอนเกมจบ พร้อมส่งคะแนนมา
    // ══════════════════════════════════════════════════
    public void Show(int score)
    {
        gameObject.SetActive(true);
        _score     = score;
        _submitted = false;

        scoreText.text        = "คะแนน: " + score.ToString("N0");
        statusText.text       = "";
        submitButton.interactable = true;
    }

    // ══════════════════════════════════════════════════
    //  SUBMIT
    // ══════════════════════════════════════════════════
    public void OnSubmit()
    {
        if (_submitted) return;

        string username = PlayerPrefs.GetString("Username", "Player");
        SetStatus("กำลังส่งคะแนน...");
        submitButton.interactable = false;

        LootLockerManager.Instance.SubmitScore(
            username: username,
            score:    _score,
            onSuccess: () =>
            {
                _submitted = true;
                SetStatus("ส่งคะแนนสำเร็จ ✓");
                rankingButton.interactable = true;
            },
            onFail: (err) =>
            {
                SetStatus("ส่งไม่สำเร็จ: " + err);
                submitButton.interactable = true;
            }
        );
    }

    // ══════════════════════════════════════════════════
    //  VIEW RANKING
    // ══════════════════════════════════════════════════
    public void OnViewRanking()
    {
        leaderboardPanel.SetActive(true);
        leaderboardPanel.GetComponent<LeaderboardSceneUI>().Load();
    }

    // ══════════════════════════════════════════════════
    //  RETRY / MENU
    // ══════════════════════════════════════════════════
    public void OnRetry() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void OnMenu()  => SceneManager.LoadScene("MainMenu");

    void SetStatus(string msg) => statusText.text = msg;
}