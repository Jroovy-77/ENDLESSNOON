using UnityEngine;

public class RagdollPlayerController : MonoBehaviour
{
    [Header("Main Rigidbody (Hips)")]
    public Rigidbody hips;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveForce = 250f;
    public float maxSpeed = 4f;
    public float rotationSpeed = 6f;

    [Header("Balance / Wobble")]
    public float uprightForce = 400f;   // แรงพยุง
    public float wobbleForce = 60f;     // แรงโคลง
    public float fallAngle = 45f;       // มุมที่ถือว่าล้ม

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool isFallen = false;

    void FixedUpdate()
    {
        if (hips == null) return;

        CheckGround();
        Move();
        LimitSpeed();

        if (!isFallen)
        {
            WobbleAndBalance(); // พยายามยืน + โคลง
            CheckFall();        // ตรวจว่าล้มยัง
        }
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );
    }

    void Move()
    {
        if (cameraTransform == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        if (moveDir.magnitude > 0.1f && isGrounded)
        {
            hips.AddForce(moveDir * moveForce);

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            hips.MoveRotation(Quaternion.Slerp(
                hips.rotation,
                targetRot,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(hips.velocity.x, 0, hips.velocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            hips.velocity = new Vector3(limited.x, hips.velocity.y, limited.z);
        }
    }

    void WobbleAndBalance()
    {
        // พยุงให้ตั้ง
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
        hips.AddTorque(torque * uprightForce);

        // ใส่ความโคลง
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        );

        hips.AddTorque(randomTorque * wobbleForce);
    }

    void CheckFall()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);

        if (angle > fallAngle)
        {
            isFallen = true; // ปล่อยให้ล้มจริง
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}