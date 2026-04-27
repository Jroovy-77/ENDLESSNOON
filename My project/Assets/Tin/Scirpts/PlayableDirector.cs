using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneFadeToScene : MonoBehaviour
{
    public PlayableDirector director;
    public CanvasGroup fadePanel;

    [Header("Next Scene")]
    public string nextSceneName = "2";

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private bool isLoading = false;

    void Start()
    {
        if (fadePanel != null)
            fadePanel.alpha = 0f;

        if (director != null)
        {
            director.stopped += OnCutsceneFinished;
            director.Play();
        }
    }

    void Update()
    {
        // กด E เพื่อ Skip
        if (!isLoading && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SkipAndLoad());
        }
    }

    void OnCutsceneFinished(PlayableDirector pd)
    {
        if (!isLoading)
            StartCoroutine(FadeAndLoad());
    }

    IEnumerator SkipAndLoad()
    {
        isLoading = true;

        if (director != null)
        {
            director.stopped -= OnCutsceneFinished;
            director.Stop();
        }

        yield return StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            yield return null;
        }

        fadePanel.alpha = 1f;

        // เปลี่ยนฉาก
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnCutsceneFinished;
    }
}