using UnityEngine;

public class FallWhenPlayerEnter : MonoBehaviour
{
    [Header("Player (ลากใส่ Inspector)")]
    public Transform player;

    [Header("Detection Box")]
    public Vector3 boxSize = new Vector3(3f, 2f, 3f);

    [Header("Fall Rotation")]
    public Vector3 fallRotation = new Vector3(90f, 0f, 0f);
    public float smoothSpeed = 5f;

    private Quaternion targetRot;
    private bool hasFallen = false;

    void Start()
    {
        targetRot = Quaternion.Euler(fallRotation);
    }

    void Update()
    {
        if (player == null || hasFallen) return;

        if (IsPlayerInsideBox())
        {
            hasFallen = true; // ล้มครั้งเดียวแล้วล็อก
        }

        if (hasFallen)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * smoothSpeed
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
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}