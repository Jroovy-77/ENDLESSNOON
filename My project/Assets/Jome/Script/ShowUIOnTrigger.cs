using UnityEngine;

public class ShowUIByDistance : MonoBehaviour
{
    [Header("ลาก Player มาใส่")]
    public Transform player;

    [Header("ลาก UI มาใส่")]
    public GameObject uiObject;

    [Header("ระยะที่ให้ UI โผล่")]
    public float showDistance = 3f;

    void Start()
    {
        if (uiObject != null)
            uiObject.SetActive(false);
    }

    void Update()
    {
        if (player == null || uiObject == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= showDistance)
            uiObject.SetActive(true);
        else
            uiObject.SetActive(false);
    }
}