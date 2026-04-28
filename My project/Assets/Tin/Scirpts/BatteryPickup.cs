using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!GameManager.Instance.missionStarted) return; // ยังไม่ start หยิบไม่ได้

        if (other.CompareTag("Player"))
        {
            collected = true;
            GameManager.Instance.AddBattery();
            Destroy(gameObject);
        }
    }
}