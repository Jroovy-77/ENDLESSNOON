using UnityEngine;

public class RagdollPlayerController : MonoBehaviour
{
    [Header("Main Rigidbody")]
    public Rigidbody hips;
    public Rigidbody chest;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveForce = 250f;
    public float maxSpeed = 4f;
    public float rotationSpeed = 6f;

    [Header("Balance")]
    public float uprightForce = 250f;

    [Header("Wobble")]
    public float idleWobble = 10f;
    public float moveWobble = 30f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    bool isGrounded;

    // 🔥 ตัวแก้สั่น
    float wobbleTimer = 0f;
    Vector3 wobbleDir;

    void FixedUpdate()
    {
        if (hips == null) return;

        CheckGround();

        Vector3 inputDir = GetMoveInput();

        Move(inputDir);
        LimitSpeed();

        Balance();
        Wobble(inputDir);
    }

    // ===================== INPUT =====================
    Vector3 GetMoveInput()
    {
        if (cameraTransform == null) return Vector3.zero;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        return (camForward * v + camRight * h).normalized;
    }

    // ===================== MOVE =====================
    void Move(Vector3 dir)
    {
        if (dir.magnitude > 0.1f && isGrounded)
        {
            hips.AddForce(dir * moveForce);

            Quaternion targetRot = Quaternion.LookRotation(dir);
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

    // ===================== BALANCE =====================
    void Balance()
    {
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
        hips.AddTorque(torque * uprightForce);
    }

    // ===================== WOBBLE (แก้สั่น) =====================
    void Wobble(Vector3 inputDir)
    {
        if (chest == null) return;

        // 🔥 เปลี่ยนทิศ wobble เป็นช่วงๆ ไม่ใช่ทุกเฟรม
        wobbleTimer -= Time.fixedDeltaTime;

        if (wobbleTimer <= 0f)
        {
            wobbleTimer = Random.Range(0.5f, 1.2f);

            wobbleDir = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized;
        }

        // 🔥 ถ้ายืนเฉย = โคลงน้อย
        float wobbleForce = (inputDir.magnitude < 0.1f) ? idleWobble : moveWobble;

        chest.AddTorque(wobbleDir * wobbleForce);
    }

    // ===================== GROUND =====================
    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );
    }
}