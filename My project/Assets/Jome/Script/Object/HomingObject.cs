using UnityEngine;

public class HomingObject : MonoBehaviour
{
    public Transform player;      // ลากเอาตัว Player มาใส่
    public float speed = 10f;     // ความเร็วในการพุ่งใส่

    void Update()
    {
        if (player != null)
        {
            // คำนวณให้พุ่งไปที่ตำแหน่ง Player โดยตรง
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            
            // แถม: ให้หันหน้าไปทาง Player ด้วย
            transform.LookAt(player);
        }
    }
}