using UnityEngine;
using UnityEngine.Playables;

public class ShipDoor : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject player;
    public Camera gameplayCamera;
    public Camera cutsceneCamera;

    private bool played = false;

    private void Start()
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
            GameManager.Instance.ShowMessage("Talk to IronMan first!");
            return;
        }

        played = true;

        if (player != null)
            player.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);

        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(true);

        if (director != null)
            director.Play();
    }
}