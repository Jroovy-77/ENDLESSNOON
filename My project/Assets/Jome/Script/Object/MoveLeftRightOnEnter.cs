using UnityEngine;

public class MoveLeftRightOnceOnEnter : MonoBehaviour
{
    [Header("Player (ลากใส่ Inspector)")]
    public Transform player;

    [Header("Detection Box Size")]
    public Vector3 boxSize = new Vector3(3f, 2f, 3f);

    [Header("Move Settings")]
    public float moveDistance = 2f;
    public float moveSpeed = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool hasMoved = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + transform.right * moveDistance;
    }

    void Update()
    {
        if (player == null) return;

        if (!hasMoved && IsPlayerInsideBox())
        {
            hasMoved = true; // ใช้ครั้งเดียว
        }

        if (hasMoved)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * moveSpeed
            );
        }
    }

    bool IsPlayerInsideBox()
    {
        Vector3 localPos = transform.InverseTransformPoint(player.position);
        Vector3 half = boxSize * 0.5f;

        return Mathf.Abs(localPos.x) <= half.x &&
               Mathf.Abs(localPos.y) <= half.y &&
               Mathf.Abs(localPos.z) <= half.z;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}