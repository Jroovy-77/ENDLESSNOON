using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ShipDoor : MonoBehaviour
{
    [Header("Cutscene")]
    public PlayableDirector director;

    [Header("Player")]
    public GameObject player;

    [Header("Camera")]
    public Camera gameplayCamera;
    public Camera cutsceneCamera;

    [Header("Audio")]
    public AudioSource bgmSource;

    [Header("Fade")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1.5f;

    [Header("Next Scene")]
    public string nextSceneName = "NextScene";

    private bool played = false;

    void Start()
    {
        // เปิดเมาส์ในซีนนี้
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (director != null)
        {
            director.playOnAwake = false;
            director.Stop();
            director.stopped += OnCutsceneFinished;
        }

        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(false);

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (played) return;
        if (!other.CompareTag("Player")) return;

        // ยังไม่ผ่านภารกิจ
        if (!GameManager.Instance.canEnterShip)
        {
            GameManager.Instance.ShowMessage("Finish the mission first!");
            return;
        }

        played = true;

        if (player == null)
            player = other.gameObject;

        StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        // หยุดเพลง
        if (bgmSource != null)
            bgmSource.Stop();

        // ปิดการควบคุม player ก่อน
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(0.1f);

        // ซ่อน player
        player.SetActive(false);

        // ปิดกล้องเกมเพลย์
        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);

        // เปิดกล้องคัตซีน
        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(true);

        // เล่นคัตซีน
        if (director != null)
            director.Play();
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            if (fadeGroup != null)
                fadeGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

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