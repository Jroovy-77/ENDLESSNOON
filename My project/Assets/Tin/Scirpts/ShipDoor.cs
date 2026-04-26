using UnityEngine;
using UnityEngine.Playables;

public class ShipDoor : MonoBehaviour
{
    public PlayableDirector director;

    public GameObject player;
    public Camera gameplayCamera;
    public Camera cutsceneCamera;

    public AudioSource bgmSource; // เพลงพื้นหลัง

    private bool played = false;

    void Start()
    {
        if (director != null)
        {
            director.playOnAwake = false;
            director.Stop();
        }

        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (played) return;
        if (!other.CompareTag("Player")) return;

        if (!GameManager.Instance.canEnterShip)
        {
            GameManager.Instance.ShowMessage("Finish the mission first!");
            return;
        }

        played = true;

        // ปิดเพลงพื้นหลัง
        if (bgmSource != null)
            bgmSource.Stop();

        // ปิด player / กล้องเกม
        if (player != null)
            player.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);

        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(true);

        // เล่นคัตซีน
        if (director != null)
            director.Play();
    }
}