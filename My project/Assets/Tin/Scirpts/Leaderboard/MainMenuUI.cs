using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Scene: MainMenu
/// แสดงชื่อ, ปุ่ม Play, ปุ่ม Leaderboard, ปุ่ม Leave
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public string gameSceneName        = "GameScene";
    public string leaderboardSceneName = "Leaderboard";

    void Start()
    {
        string username = PlayerPrefs.GetString("Username", "Player");
        if (usernameText != null)
            usernameText.text = "Hello, " + username;
    }

    public void OnPlay()        => SceneManager.LoadScene(gameSceneName);
    public void OnLeaderboard() => SceneManager.LoadScene(leaderboardSceneName);
    public void OnLeave()       => Application.Quit();
}