using UnityEngine;

public class AutoRamAttack : MonoBehaviour
{
    public string targetTag = "Enemy"; // แท็กของเป้าหมายที่ต้องการพุ่งชน
    public float detectionRadius = 10f; // ระยะตรวจจับ
    public float ramForce = 25f;        // ความแรงในการพุ่ง
    
    private Rigidbody rb;
    private bool isRamming = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ตัวอย่าง: กด Space เพื่อสแกนหาเป้าหมายและพุ่งชน
        if (Input.GetKeyDown(KeyCode.Space) && !isRamming)
        {
            TryRamTarget();
        }
    }

    void TryRamTarget()
    {
        // 1. ค้นหาวัตถุทั้งหมดที่มี Collider ในระยะ detectionRadius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            // 2. เช็คว่ามี Tag ที่เราต้องการไหม
            if (hitCollider.CompareTag(targetTag))
            {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                // 3. หาตัวที่อยู่ใกล้ที่สุด
                if (distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    closestTarget = hitCollider.transform;
                }
            }
        }

        // 4. ถ้าเจอเป้าหมาย ให้ทำการพุ่งใส่
        if (closestTarget != null)
        {
            Vector3 direction = (closestTarget.position - transform.position).normalized;
            StartCoroutine(PerformRam(direction));
        }
    }

    System.Collections.IEnumerator PerformRam(Vector3 direction)
    {
        isRamming = true;

        // หันหน้าไปทางที่จะพุ่ง (Optional)
        transform.forward = new Vector3(direction.x, 0, direction.z);

        rb.velocity = Vector3.zero; 
        rb.AddForce(direction * ramForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.6f); // ระยะเวลาพุ่ง
        isRamming = false;
    }

    // วาดวงกลมใน Scene View เพื่อให้เห็นระยะตรวจจับ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}