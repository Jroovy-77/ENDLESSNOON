using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{
    [Header("Cutscene")]
    public PlayableDirector director;

    [Header("Next Scene")]
    public string nextSceneName = "GameScene";

    private bool skipped = false;

    void Update()
    {
        // กด E เพื่อ Skip
        if (!skipped && Input.GetKeyDown(KeyCode.E))
        {
            SkipCutscene();
        }

        // ถ้าคัตซีนเล่นจบเอง
        if (!skipped && director != null && director.state != PlayState.Playing)
        {
            skipped = true;
            LoadNextScene();
        }
    }

    public void SkipCutscene()
    {
        skipped = true;

        if (director != null)
        {
            director.Stop();
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}