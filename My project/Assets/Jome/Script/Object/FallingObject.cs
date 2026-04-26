using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Collision Settings")]
    public string groundLayerName = "Ground"; // ชื่อเลเยอร์พื้น
    public string playerLayerName = "Player"; // ชื่อเลเยอร์คน

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        rb.useGravity = false;
    }

    public void Release()
    {
        // 1. หาหมายเลข Layer จากชื่อที่เราตั้งไว้
        int groundLayer = LayerMask.NameToLayer(groundLayerName);
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int myLayer = gameObject.layer;

        // 2. สั่งให้ "มองข้าม" การชนกับทุก Layer ยกเว้นพื้นกับคน
        // วนลูปทุก Layer (ใน Unity มี 32 เลเยอร์)
        for (int i = 0; i < 32; i++)
        {
            // ถ้าเลเยอร์นั้นไม่ใช่พื้น และไม่ใช่คน ให้ ignore (ทะลุ) ไปเลย
            if (i != groundLayer && i != playerLayer)
            {
                Physics.IgnoreLayerCollision(myLayer, i, true);
            }
            else
            {
                // ถ้าเป็นพื้นหรือคน ให้ชนได้ปกติ
                Physics.IgnoreLayerCollision(myLayer, i, false);
            }
        }

        // 3. ปล่อยหินตก
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
}