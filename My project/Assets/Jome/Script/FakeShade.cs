using UnityEngine;

public class FakeShade : MonoBehaviour
{
    public float minScaleMultiplier = 0.2f;
    public float scaleSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // scale
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );

        // position
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * scaleSpeed
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shrink();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Grow();
        }
    }

    void Shrink()
    {
        float newY = originalScale.y * minScaleMultiplier;

        targetScale = new Vector3(
            originalScale.x,
            newY,
            originalScale.z
        );

        // 🔥 ขยับขึ้นครึ่งนึงของที่หายไป
        float heightDiff = originalScale.y - newY;
        targetPosition = originalPosition - new Vector3(0, heightDiff / 2f, 0);
    }

    void Grow()
    {
        targetScale = originalScale;
        targetPosition = originalPosition;
    }
}