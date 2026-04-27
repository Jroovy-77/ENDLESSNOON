using UnityEngine;
using UnityEngine.UI;

public class TalkTrigger : MonoBehaviour
{
    public float      talkRange = 3f;
    public GameObject talkUI;    // UI ใน Screen Space Overlay Canvas
    public Vector3    offset = new Vector3(0, 100f, 0); // เลื่อนขึ้นเหนือหัว

    private Transform _player;
    private Camera    _cam;

    void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _cam    = Camera.main;
        if (talkUI != null) talkUI.SetActive(false);
    }

    void Update()
    {
        if (_player == null) return;

        bool inRange = Vector3.Distance(transform.position, _player.position) <= talkRange;

        if (talkUI != null)
        {
            talkUI.SetActive(inRange);

            if (inRange)
            {
                // แปลง World Position ของ Object → Screen Position
                Vector3 screenPos = _cam.WorldToScreenPoint(transform.position);
                talkUI.GetComponent<RectTransform>().position = screenPos + offset;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }
}