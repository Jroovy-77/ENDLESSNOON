using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            GameManager.Instance.AddBattery();
            Destroy(gameObject);
        }
    }
}