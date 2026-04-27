using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Scene: EnterName
/// ใส่ชื่อ → กด Confirm → ไป MainMenu
/// </summary>
public class EnterNameUI : MonoBehaviour
{
    public TMP_InputField  usernameInput;
    public TextMeshProUGUI statusText;
    public Button          confirmButton;

    void Start()
    {
        // โหลดชื่อเดิมถ้ามี
        usernameInput.text = PlayerPrefs.GetString("Username", "");
        SetStatus("Connecting...");
        confirmButton.interactable = false;

        LootLockerManager.Instance.Login(
            onSuccess: () =>
            {
                SetStatus("");
                confirmButton.interactable = true;
            },
            onFail: (err) => SetStatus("Failed to connect : " + err)
        );  
    }

    public void OnConfirm()
    {
        string username = usernameInput.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            SetStatus("Input Your Name");
            return;
        }

        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();

        SceneManager.LoadScene("StartGame");
    }

    void SetStatus(string msg) => statusText.text = msg;
}