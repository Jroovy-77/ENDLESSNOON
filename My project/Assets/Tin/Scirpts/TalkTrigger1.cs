using UnityEngine;

public class TalkTrigger1 : MonoBehaviour
{
    public float      talkRange = 3f;
    public GameObject talkUI;
    public Vector3    offset = new Vector3(0, 100f, 0);

    private Transform _player;
    private Camera    _cam;

    void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _cam    = Camera.main;

        Debug.Log("Player: " + (_player != null ? _player.name : "NULL"));
        Debug.Log("talkUI: " + (talkUI != null ? talkUI.name : "NULL"));
        Debug.Log("Camera: " + (_cam != null ? _cam.name : "NULL"));

        if (talkUI != null) talkUI.SetActive(false);
    }

    void Update()
    {
        if (_player == null || talkUI == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= talkRange;

        Debug.Log("dist: " + dist + " inRange: " + inRange);

        talkUI.SetActive(inRange);

        if (inRange)
        {
            Vector3 screenPos = _cam.WorldToScreenPoint(transform.position);
            talkUI.GetComponent<RectTransform>().position = screenPos + offset;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }
}