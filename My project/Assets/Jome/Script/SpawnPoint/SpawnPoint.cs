using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static Vector3 spawnPos;
    public static Quaternion spawnRot;
    public static bool hasSpawn;

    void Awake()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
        hasSpawn = true;
    }
}