using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Collision Settings")]
    public string groundLayerName = "Ground";
    public string playerLayerName = "Player";

    [Header("Fall Speed Settings")]
    public float initialDownForce = 15f; // แรงถีบลงจังหวะแรก (ยิ่งเยอะยิ่งพุ่ง)
    public float extraGravity = 20f;     // แรงดึงดูดพิเศษ (ทำให้ตกเร็วขึ้นเรื่อยๆ แบบไม่ลอยชาย)
    
    private bool isReleased = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        rb.useGravity = false;
        
        // แนะนำให้ปรับ Collision Detection เป็น Continuous เพื่อกันหินพุ่งทะลุพื้นเพราะความเร็วสูง
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        // ถ้าถูกปล่อยแล้ว ให้เพิ่มแรงดึงดูดพิเศษลงไปตลอดเวลา
        if (isReleased)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    public void Release()
    {
        if (isReleased) return; // กันการกดซ้ำ

        // 1. ระบบ Ignore Collision (ทะลุทุกอย่างยกเว้นพื้นและคน)
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

        // 2. ปล่อยหิน
        isReleased = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        // 3. ใส่แรงถีบจังหวะแรกสุด
        rb.AddForce(Vector3.down * initialDownForce, ForceMode.Impulse);
        
        Debug.Log("ปล่อยหินด้วยความเร็วสูง!");
    }
}