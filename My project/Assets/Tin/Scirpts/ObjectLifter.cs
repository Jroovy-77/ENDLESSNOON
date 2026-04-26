using UnityEngine;

public class ObjectLifter : MonoBehaviour
{
    public float pickupRange = 3f;
    public float holdHeight  = 1.5f;
    public float holdForward = 0.5f;
    public string liftableTag = "Liftable";
    public string trapTag     = "Trap";

    private GameObject _held;
    private Rigidbody  _heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_held) Drop();
            else       TryPickup();
        }

        if (_held != null)
        {
            Vector3 target = transform.position
                           + Vector3.up        * holdHeight
                           + transform.forward * holdForward;
            _held.transform.position = target;
        }
    }

    // พอชน Trap → วางของทันที
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(trapTag) && _held != null)
            Drop();
    }

    void TryPickup()
    {
        int layerMask = ~(1 << gameObject.layer);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, pickupRange, layerMask)) return;
        if (!hit.collider.CompareTag(liftableTag)) return;

        Rigidbody rb = hit.rigidbody;
        if (rb == null) return;

        _held   = hit.collider.gameObject;
        _heldRb = rb;

        _heldRb.isKinematic = true;
        _heldRb.useGravity  = false;

        foreach (var hc in _held.GetComponentsInChildren<Collider>())
            foreach (var pc in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(hc, pc, true);
    }

    void Drop()
    {
        foreach (var hc in _held.GetComponentsInChildren<Collider>())
            foreach (var pc in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(hc, pc, false);

        _heldRb.isKinematic = false;
        _heldRb.useGravity  = true;

        _held   = null;
        _heldRb = null;
    }
}