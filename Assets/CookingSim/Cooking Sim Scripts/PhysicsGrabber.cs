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

    private Rigidbody heldObject;
    private SpringJoint grabJoint;
    [SerializeField] private GameObject holdTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Grab();
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

                //important to set the position before parenting, otherwise the object will snap to the holdTarget's position
                holdTarget.transform.position = hit.point;

                // adds sluggish so it doesn't freak out
                float originalDrag = rb.linearDamping;
                float originalAngularDrag = rb.angularDamping;
                rb.linearDamping = holdDrag;
                rb.angularDamping = holdAngularDrag;
            

                grabJoint = holdTarget.GetComponent<SpringJoint>();
                if (grabJoint != null)
                {
                    grabJoint.connectedBody = heldObject;
                }
                else
                {
                    Debug.LogError("Object doesnt have Spring Joint");
                }
                
                // off set
                grabJoint.autoConfigureConnectedAnchor = false;
                grabJoint.anchor = Vector3.zero; 
                grabJoint.connectedAnchor = heldObject.transform.InverseTransformPoint(hit.point);

                // applies forces based on weight
                grabJoint.spring = springForce * heldObject.mass;
                grabJoint.damper = damper * heldObject.mass;
                grabJoint.maxDistance = 0f;
                grabJoint.minDistance = 0f;
                
                Debug.Log("grabbed: " + heldObject.name);
            }
        }
    }
}