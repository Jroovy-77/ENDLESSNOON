using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Battery")]
    public int batteryCount = 0;
    public int maxBattery = 3;

    [Header("Mission")]
    public bool missionStarted = false;
    public bool missionComplete = false;
    public bool canEnterShip = false;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    // เริ่มเควส
    public void StartMission()
    {
        missionStarted = true;
        ShowMessage("Locate 3 Power Cells and restore the ship.");
    }

    // เก็บแบต
    public void AddBattery()
    {
        if (!missionStarted) return;

        batteryCount++;

        if (batteryCount > maxBattery)
            batteryCount = maxBattery;

        if (batteryCount == 1)
        {
            ShowMessage("First Power Cell secured.");
        }
        else if (batteryCount == 2)
        {
            ShowMessage("Good. One more remains.");
        }
        else if (batteryCount >= maxBattery)
        {
            missionComplete = true;
            ShowMessage("All Power Cells secured. \nReport back immediately.");
        }
    }

    // ส่งเควส
    public void AllowEnterShip()
    {
        canEnterShip = true;
        ShowMessage("Ship ready. Board immediately.");
    }

    public void ShowMessage(string msg)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = msg;

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 3f);
    }

    void HideMessage()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}