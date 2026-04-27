using UnityEngine;

public class DestroyPlayerOnCutsceneStart : MonoBehaviour
{
    public GameObject player; // ลาก Player มาใส่

    void Start()
    {
        if (player != null)
        {
            Destroy(player);
        }
    }
}