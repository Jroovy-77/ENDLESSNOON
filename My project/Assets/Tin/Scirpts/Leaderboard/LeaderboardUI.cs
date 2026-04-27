using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;
using System.Collections;

public class LeaderboardSceneUI : MonoBehaviour
{
    public Transform       entryContainer;
    public GameObject      entryPrefab;
    public TextMeshProUGUI statusText;
    public Button          refreshButton;
    public string          mainMenuScene = "StartGame";

    void Start() => StartCoroutine(WaitThenLoad());

    IEnumerator WaitThenLoad()
    {
        SetStatus("Conecting...");

        // รอจนกว่า LootLocker จะ Ready
        float timeout = 10f;
        while (!LootLockerManager.Instance.IsReady && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (!LootLockerManager.Instance.IsReady)
        {
            // ยัง Login ไม่สำเร็จ ลอง Login ใหม่
            LootLockerManager.Instance.Login(
                onSuccess: Load,
                onFail: (err) => SetStatus("Faild to Conect: " + err)
            );
        }
        else
        {
            Load();
        }
    }

    public void Load()
    {
        SetStatus("Conecting...");
        if (refreshButton != null) refreshButton.interactable = false;

        foreach (Transform child in entryContainer)
            Destroy(child.gameObject);

        LootLockerManager.Instance.GetLeaderboard(
            onSuccess: (items) =>
            {
                if (items == null || items.Length == 0)
                {
                    SetStatus("No Score");
                    if (refreshButton != null) refreshButton.interactable = true;
                    return;
                }

                foreach (var item in items)
                {
                    string name = (item.player != null && !string.IsNullOrEmpty(item.player.name))
                                  ? item.player.name : item.member_id;
                    SpawnEntry(item.rank, name, item.score);
                }

                SetStatus("");
                if (refreshButton != null) refreshButton.interactable = true;
            },
            onFail: (err) =>
            {
                SetStatus("โหลดไม่สำเร็จ: " + err);
                if (refreshButton != null) refreshButton.interactable = true;
            }
        );
    }

    void SpawnEntry(int rank, string username, int score)
    {
        GameObject entry = Instantiate(entryPrefab, entryContainer);
        TextMeshProUGUI tmp = entry.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = entry.GetComponentInChildren<TextMeshProUGUI>();

        if (tmp != null)
        {
            tmp.text = $"{rank,-20}{username,-20}{score,10}";

            string myName = PlayerPrefs.GetString("Username", "");
            tmp.color = (username == myName) ? new Color(0f, 0f, 0f) : Color.white;
        }
    }

    public void OnRefresh() => StartCoroutine(WaitThenLoad());
    public void OnBack()    => SceneManager.LoadScene(mainMenuScene);

    void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}