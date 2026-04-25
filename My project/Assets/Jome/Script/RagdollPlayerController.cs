using UnityEngine;

public class RagdollPlayerController : MonoBehaviour
{
    [Header("Main Body")]
    public Rigidbody body;              // ส่วนสะโพก (Hips)
    public Transform cameraTransform;   
    public Animator anim;               // 🔥 ลาก Animator ของตัวละครมาใส่ที่นี่

    [Header("Movement")]
    public float moveForce = 40f;       
    public float turnSpeed = 15f;       

    [Header("Balance")]
    public float uprightForce = 500f;   
    public float balanceDamping = 10f;  

    void FixedUpdate()
    {
        Move();
        StayUpright();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 1. คำนวณทิศทางตามกล้อง
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 🔥 [เพิ่มส่วนนี้] ส่งค่าไปยัง Animator
        if (anim != null)
        {
            // ใช้ความยาวของ moveDir (0 หรือ 1) ส่งไปที่พารามิเตอร์ Speed
            // 0.1f คือค่า Smooth ป้องกันอนิเมชั่นกระตุก
            anim.SetFloat("Speed", moveDir.magnitude, 0.1f, Time.fixedDeltaTime);
        }

        if (moveDir.magnitude > 0.1f)
        {
            // 2. หมุนตัวละครให้หันไปทางที่เดิน (Roblox Style)
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            body.MoveRotation(Quaternion.Slerp(body.rotation, targetRot, Time.fixedDeltaTime * turnSpeed));

            // 3. ออกแรงเดิน
            body.AddForce(moveDir * moveForce, ForceMode.Acceleration);
        }
    }

    void StayUpright()
    {
        Vector3 torque = Vector3.Cross(body.transform.up, Vector3.up);
        body.AddTorque(torque * uprightForce, ForceMode.Acceleration);

        Vector3 antiSpin = -body.angularVelocity * balanceDamping;
        body.AddTorque(antiSpin, ForceMode.Acceleration);
    }
}