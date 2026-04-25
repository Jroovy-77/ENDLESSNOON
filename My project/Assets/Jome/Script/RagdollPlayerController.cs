using UnityEngine;

public class RagdollPlayerController : MonoBehaviour
{
    [Header("Main Body")]
    public Rigidbody body;              
    public Transform cameraTransform;   

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

        // 1. หาหน้าตรงของกล้องในระนาบพื้น (ตัดแกน Y)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // 2. ทิศทางที่กดปุ่ม (W,A,S,D)
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            // --- ส่วนที่แก้ให้เหมือน Roblox ---
            
            // 3. หมุนตัวละครให้หันไปทาง 'ทิศที่เดิน' (ซึ่งอิงจากกล้องแล้ว)
            // ทิศนี้จะทำให้หน้าตัวละครหันออกจากกล้องเสมอเวลากด W
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            
            // ใช้ MoveRotation เพื่อความนิ่งของ Ragdoll
            body.MoveRotation(Quaternion.Slerp(body.rotation, targetRot, Time.fixedDeltaTime * turnSpeed));

            // 4. ออกแรงเดิน
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