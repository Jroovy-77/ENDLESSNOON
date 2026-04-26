using UnityEngine;

public class RiseWhenPlayerNear : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Box Range")]
    public Vector3 boxSize = new Vector3(5f, 3f, 5f); // ขนาดกล่องตรวจจับ (กว้าง, สูง, ลึก)

    [Header("Rise Settings")]
    public float riseHeight = 3f;
    public float riseSpeed = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private bool isRising = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;
    }

    void Update()
    {
        if (player == null) return;

        // เช็คว่า Player อยู่ในกล่องหรือไม่
        if (IsInsideBox(player.position))
        {
            isRising = true;
        }

        if (isRising)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                riseSpeed * Time.deltaTime
            );
        }
    }

    bool IsInsideBox(Vector3 point)
    {
        Vector3 localPos = point - transform.position;

        return Mathf.Abs(localPos.x) <= boxSize.x / 2 &&
               Mathf.Abs(localPos.y) <= boxSize.y / 2 &&
               Mathf.Abs(localPos.z) <= boxSize.z / 2;
    }

    // วาดกล่องใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}