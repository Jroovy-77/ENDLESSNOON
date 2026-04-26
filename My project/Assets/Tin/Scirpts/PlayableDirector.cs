using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneFadeToScene : MonoBehaviour
{
    public PlayableDirector director;
    public CanvasGroup fadePanel;

    public string nextSceneName = "NextScene";
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (director != null)
        {
            director.stopped += OnCutsceneFinished;
            director.Play();
        }

        // กันไว้ก่อนให้จอดำเริ่มโปร่ง
        if (fadePanel != null)
            fadePanel.alpha = 0f;
    }

    void OnCutsceneFinished(PlayableDirector pd)
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // Fade to black
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // เปลี่ยนซีน
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnCutsceneFinished;
    }
}