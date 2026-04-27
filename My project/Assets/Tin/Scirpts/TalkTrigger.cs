using UnityEngine;

public class TalkTrigger : MonoBehaviour
{
    public float     talkRange = 3f;
    public GameObject talkUI;   // UI ลอยเหนือ Object

    private Transform _player;

    void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        if (talkUI != null) talkUI.SetActive(false);
    }

    void Update()
    {
        if (_player == null) return;

        bool inRange = Vector3.Distance(transform.position, _player.position) <= talkRange;
        if (talkUI != null) talkUI.SetActive(inRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }
}