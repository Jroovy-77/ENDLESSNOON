using UnityEngine;

public class GrabSystem : MonoBehaviour
{
    [Header("Settings")]
    public Rigidbody handRb;          // กระดูกมือ
    public float grabRadius = 0.5f;   
    public KeyCode grabKey = KeyCode.E; 

    [Header("Lift Position")]
    public Vector3 liftOffset = new Vector3(0, 1.5f, 0.5f); // ตำแหน่งที่จะให้ของไปอยู่ (เหนือหัวและไปข้างหน้าหน่อยๆ)

    [Header("Filters")]
    public GameObject playerRoot;     

    private ConfigurableJoint grabJoint;
    private Rigidbody grabbedObject;

    void Update()
    {
        if (Input.GetKeyDown(grabKey)) TryGrab();
        if (Input.GetKeyUp(grabKey)) Release();
    }

    void TryGrab()
    {
        if (grabJoint != null) return;

        Collider[] hits = Physics.OverlapSphere(handRb.position, grabRadius);
        foreach (var hit in hits)
        {
            Rigidbody targetRb = hit.attachedRigidbody;
            
            if (targetRb != null && targetRb.transform.root != playerRoot.transform.root)
            {
                grabbedObject = targetRb;

                // --- ทริคพิเศษ: วาร์ปของไปตำแหน่งเหนือหัวก่อนสร้าง Joint ---
                // วิธีนี้จะกระชากแขน Ragdoll ให้ยกขึ้นทันที
                Vector3 targetPos = playerRoot.transform.position + playerRoot.transform.up * liftOffset.y + playerRoot.transform.forward * liftOffset.z;
                grabbedObject.transform.position = targetPos;

                // สร้าง Joint เพื่อล็อคไว้กับมือ
                grabJoint = handRb.gameObject.AddComponent<ConfigurableJoint>();
                grabJoint.connectedBody = grabbedObject;

                // ล็อคให้ติดหนึบ
                grabJoint.xMotion = ConfigurableJointMotion.Locked;
                grabJoint.yMotion = ConfigurableJointMotion.Locked;
                grabJoint.zMotion = ConfigurableJointMotion.Locked;
                
                // ยอมให้หมุนได้บ้างจะได้ไม่ดูแข็งทื่อ
                grabJoint.angularXMotion = ConfigurableJointMotion.Free;
                grabJoint.angularYMotion = ConfigurableJointMotion.Free;
                grabJoint.angularZMotion = ConfigurableJointMotion.Free;

                break; 
            }
        }
    }

    void Release()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
            grabbedObject = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerRoot != null)
        {
            Gizmos.color = Color.green;
            Vector3 targetPos = playerRoot.transform.position + playerRoot.transform.up * liftOffset.y + playerRoot.transform.forward * liftOffset.z;
            Gizmos.DrawWireCube(targetPos, new Vector3(0.5f, 0.1f, 0.5f));
        }
    }
}