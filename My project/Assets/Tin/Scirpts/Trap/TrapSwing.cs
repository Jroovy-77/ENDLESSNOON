using UnityEngine;
using System.Collections;

public class TrapSwing : MonoBehaviour
{
    [Header("Player (ลากใส่เอง)")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Settings")]
    public float tiltSpeed = 300f;
    public float spinSpeed = 1000f;
    public float cooldown = 2f;

    private bool _ready = true;
    private Quaternion _startRot;

    void Start()
    {
        _startRot = transform.rotation;
    }

    void Update()
    {
        if (!_ready) return;
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            StartCoroutine(TreeTrapRoutine());
        }
    }

    IEnumerator TreeTrapRoutine()
    {
        _ready = false;

        // STEP 1: ล้มลง
        Quaternion flatRot = Quaternion.Euler(
            90f,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        yield return StartCoroutine(MoveRotation(flatRot, tiltSpeed));

        // STEP 2: หมุนกวาด 360
        float angleSpun = 0f;

        while (angleSpun < 360f)
        {
            float step = spinSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * step, Space.World);

            angleSpun += step;

            yield return null;
        }

        // STEP 3: กลับท่าเดิม
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(
            MoveRotation(_startRot, tiltSpeed / 2f)
        );

        yield return new WaitForSeconds(cooldown);

        _ready = true;
    }

    IEnumerator MoveRotation(Quaternion target, float speed)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = target;
    }

    void OnTriggerEnter(Collider other)
    {
        if (player != null && other.transform == player)
        {
            Debug.Log("โดนกวาดเต็มๆ!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}