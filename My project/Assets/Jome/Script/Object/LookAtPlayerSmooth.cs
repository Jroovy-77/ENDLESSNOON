using UnityEngine;

public class LookAtPlayerSmooth : MonoBehaviour
{
    public Transform target; // ลากตัวละคร (Player) มาใส่ที่นี่
    public float rotationSpeed = 5f; // ความเร็วในการหัน (ถ้าอยากให้หันทันทีให้ใส่ค่าเยอะๆ)

    void Update()
    {
        if (target == null) return;

        // 1. หาตำแหน่งเป้าหมายในระดับความสูงเดียวกับตัวเอง (เพื่อให้หมุนแค่ซ้าย-ขวา)
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

        // 2. คำนวณทิศทางที่ต้องหันไป
        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            // 3. สร้าง Rotation ที่ต้องการหันไป
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 4. ค่อยๆ หมุนไปหาเป้าหมาย (หรือใช้ transform.rotation = targetRotation ถ้าอยากให้หันทันที)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}