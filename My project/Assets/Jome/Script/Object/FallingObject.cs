using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    private Rigidbody rb;
    private bool isReleased = false;

    [Header("Detection Settings")]
    public Transform targetPlayer;    // ลากตัว Player มาใส่ช่องนี้
    public float detectDistance = 5f; // ระยะห่างที่หินจะตก (ปรับได้ใน Inspector)

    [Header("Collision Settings")]
    public string groundLayerName = "Ground";
    public string playerLayerName = "Player";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        rb.useGravity = false;

        // ถ้าลืมลาก Player ใส่ มันจะพยายามหาตัวที่มี Tag ว่า Player เอง
        if (targetPlayer == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) targetPlayer = player.transform;
        }
    }

    void Update()
    {
        // ถ้ายังไม่ตก และมีเป้าหมายให้ตรวจจับ
        if (!isReleased && targetPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            
            // ถ้าผู้เล่นเข้ามาใกล้กว่าระยะที่ตั้งไว้
            if (distance <= detectDistance)
            {
                Release();
            }
        }
    }

    public void Release()
    {
        if (isReleased) return;
        isReleased = true;

        // 1. จัดการ Layer Collision
        int groundLayer = LayerMask.NameToLayer(groundLayerName);
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int myLayer = gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if (i != groundLayer && i != playerLayer)
            {
                Physics.IgnoreLayerCollision(myLayer, i, true);
            }
            else
            {
                Physics.IgnoreLayerCollision(myLayer, i, false);
            }
        }

        // 2. ปล่อยหินตก
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        Debug.Log("🪨 หินหล่นใส่หัวแล้ว!");
    }

    // วาดวงรัศมีในหน้า Scene เพื่อให้เราเห็นระยะที่ตั้งไว้
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}