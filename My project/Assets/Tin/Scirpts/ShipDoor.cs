using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ShipDoor : MonoBehaviour
{
    public PlayableDirector director;

    public GameObject player;
    public Camera gameplayCamera;
    public Camera cutsceneCamera;

    public AudioSource bgmSource;

    public CanvasGroup fadeGroup;
    public float fadeDuration = 1.5f;

    [Header("Next Scene")]
    public string nextSceneName = "NextScene"; // ใส่ชื่อซีนตรงนี้

    private bool played = false;

    void Start()
    {
        if (director != null)
        {
            director.playOnAwake = false;
            director.Stop();

            // 👇 ฟังตอนคัตซีนจบ
            director.stopped += OnCutsceneFinished;
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

        if (bgmSource != null)
            bgmSource.Stop();

        if (player != null)
            player.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);

        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(true);

        if (director != null)
            director.Play();
    }

    // 👇 เรียกตอน Timeline จบ
    private void OnCutsceneFinished(PlayableDirector pd)
    {
        StartCoroutine(FadeAndLoad());
    }
    
    private IEnumerator FadeAndLoad()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeGroup != null)
                fadeGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnCutsceneFinished;
    }
}