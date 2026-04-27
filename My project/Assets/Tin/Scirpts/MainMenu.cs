using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI usernameText;

    void Start()
    {
        string username = PlayerPrefs.GetString("Username", "Player");
        if (usernameText != null)
            usernameText.text = "สวัสดี, " + username;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("StartCutSecne");
    }

    public void Mainmenu()
    {
        SceneManager.LoadScene("StartGame");
    }

    public void OnLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}