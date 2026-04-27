using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCheckpoint : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚪 ไป Scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}