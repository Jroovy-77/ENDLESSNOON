using UnityEngine;

public class NPCDirector : MonoBehaviour
{
    private bool playerNear = false;
    public GameObject talkPrompt;

    void Start()
    {
        talkPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!GameManager.Instance.missionStarted)
            {
                GameManager.Instance.StartMission();
            }
            else if (!GameManager.Instance.missionComplete)
            {
                GameManager.Instance.ShowMessage("Collect all batteries first!");
            }
            else
            {
                GameManager.Instance.AllowEnterShip();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
            talkPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
            talkPrompt.SetActive(false);    
    }
}