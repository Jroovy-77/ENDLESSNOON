using UnityEngine;

public class FootController : MonoBehaviour
{
    public Rigidbody foot;
    public Transform target; // จุดที่อยากให้เท้าไป
    public float stickForce = 300f;
    public float moveThreshold = 0.5f;

    public Transform body; // hips หรือ player

    void FixedUpdate()
    {
        if (foot == null || target == null) return;

        float speed = body.GetComponent<Rigidbody>().velocity.magnitude;

        // 🧍 ยืนเฉย → ติดพื้น
        if (speed < moveThreshold)
        {
            Vector3 force = (target.position - foot.position) * stickForce;
            foot.AddForce(force);
        }
        else
        {
            // 🚶 เดิน → ปล่อยให้ขยับ
            foot.AddForce(Vector3.down * 20f); // กันลอย
        }
    }
}