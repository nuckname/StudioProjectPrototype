using UnityEngine;

public class PhysicsGrabber : MonoBehaviour
{
    [Header("Setup")]
    public Camera playerCamera;
    public float maxGrabDistance = 15f;

    [Header("Joint Settings")]
    public float springForce = 150f;
    public float damper = 15f;
    
    [Header("Stability")]
    public float holdDrag = 10f;
    public float holdAngularDrag = 10f;

    [SerializeField] private GameObject holdTarget;
    
    private Rigidbody heldObject;
    private SpringJoint grabJoint;
    private float currentHoldDistance;

    private float originalDrag;
    private float originalAngularDrag;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Grab();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Release();
        }

        if (heldObject != null)
        {
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * currentHoldDistance;
            holdTarget.transform.position = targetPosition;
        }
    }

    void Grab()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxGrabDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                heldObject = rb;
                currentHoldDistance = hit.distance;

                //important to set the position before parenting,
                //otherwise the object will snap to the holdTarget's position
                holdTarget.transform.position = hit.point;

                originalDrag = rb.linearDamping;
                originalAngularDrag = rb.angularDamping;
                rb.linearDamping = holdDrag;
                rb.angularDamping = holdAngularDrag;

                //Spring Joint Getter
                grabJoint = holdTarget.GetComponent<SpringJoint>();

                if (grabJoint == null)
                {
                    grabJoint = holdTarget.AddComponent<SpringJoint>();
                    Debug.LogWarning("Component doesnt have sprint joint");
                }
                
                grabJoint.connectedBody = heldObject;
                
                grabJoint.autoConfigureConnectedAnchor = false;

                grabJoint.anchor = Vector3.zero; 
                
                grabJoint.connectedAnchor = heldObject.transform.InverseTransformPoint(hit.point);

                grabJoint.spring = springForce * heldObject.mass;
                grabJoint.damper = damper * heldObject.mass;
                
                grabJoint.maxDistance = 0f;
                grabJoint.minDistance = 0f;
            }
        }
    }

    void Release()
    {
        if (heldObject != null)
        {
            heldObject.linearDamping = originalDrag;
            heldObject.angularDamping = originalAngularDrag;

            if (grabJoint != null)
            {
                Destroy(grabJoint);
            }

            heldObject = null;
        }
    }
}