using UnityEngine;
using System.Collections;

public class TrapFall : MonoBehaviour
{
    public enum State { Idle, Falling, Hold }
    public State currentState = State.Idle;

    [Header("Player (ลากใส่เอง)")]
    public Transform player;

    [Header("Box Detection")]
    public float detectionWidth = 2f;
    public float detectionLength = 5f;
    public float detectionHeight = 3f;
    public float detectionOffset = 0.5f;

    [Header("Fall Settings")]
    public float fallSpeed = 90f;
    public float fallAngle = 90f;

    [Header("Timing")]
    public float holdTime = 1.5f;

    private float _currentAngle = 0f;
    private Vector3 _pivotPoint;
    private Vector3 _pivotAxis;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        float halfHeight = col != null ? col.bounds.extents.y : 1f;

        _pivotPoint = transform.position - transform.up * halfHeight;
        _pivotAxis = transform.right;
    }

    void Update()
    {
        if (currentState != State.Idle) return;
        if (player == null) return;

        if (DetectPlayer())
        {
            StartCoroutine(FallRoutine());
        }
    }

    bool DetectPlayer()
    {
        Vector3 center =
            transform.position +
            transform.forward * (detectionOffset + detectionLength * 0.5f) +
            Vector3.up * (detectionHeight * 0.5f);

        Vector3 halfSize = new Vector3(
            detectionWidth * 0.5f,
            detectionHeight * 0.5f,
            detectionLength * 0.5f
        );

        Collider[] hits = Physics.OverlapBox(
            center,
            halfSize,
            transform.rotation
        );

        foreach (var hit in hits)
        {
            if (player != null && hit.transform == player)
                return true;
        }

        return false;
    }

    IEnumerator FallRoutine()
    {
        currentState = State.Falling;

        yield return StartCoroutine(Fall());

        currentState = State.Hold;

        yield return new WaitForSeconds(holdTime);
    }

    IEnumerator Fall()
    {
        while (_currentAngle < fallAngle)
        {
            float step = fallSpeed * Time.deltaTime;
            step = Mathf.Min(step, fallAngle - _currentAngle);

            transform.RotateAround(_pivotPoint, _pivotAxis, step);

            _currentAngle += step;

            yield return null;
        }
    }

    public void ResetTrap()
    {
        StopAllCoroutines();

        transform.RotateAround(_pivotPoint, _pivotAxis, -_currentAngle);

        _currentAngle = 0f;
        currentState = State.Idle;
    }

    void OnCollisionEnter(Collision col)
    {
        if (currentState != State.Falling) return;
        if (player == null) return;

        if (col.transform == player)
        {
            Debug.Log("ล้มทับ Player!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center =
            transform.position +
            transform.forward * (detectionOffset + detectionLength * 0.5f) +
            Vector3.up * (detectionHeight * 0.5f);

        Matrix4x4 old = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            center,
            transform.rotation,
            Vector3.one
        );

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawCube(
            Vector3.zero,
            new Vector3(
                detectionWidth,
                detectionHeight,
                detectionLength
            )
        );

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(
                detectionWidth,
                detectionHeight,
                detectionLength
            )
        );

        Gizmos.matrix = old;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(
            transform.position,
            transform.forward * (detectionOffset + detectionLength)
        );
    }
}