using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Current breaking single responsibility principle, need to break up into multiple scripts
public class PlayerCollision : MonoBehaviour
{
    PlayerStealthController playerStealthController;

    [SerializeField] private Transform playerHand; 
    [SerializeField] private bool playerHasPickedUpObject;
    
    private GameObject pickedUpObject;
    private BoxCollision currentBoxData;
    
    [SerializeField] private float throwForce = 5f;
    
    private void Awake()
    {
        playerStealthController = GetComponent<PlayerStealthController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (playerHasPickedUpObject && pickedUpObject != null)
            {
                DropObject();
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Box") && !playerHasPickedUpObject)
        {
            BoxCollision box = hit.gameObject.GetComponent<BoxCollision>();
            if (box != null)
            {
                PickUpObject(hit.gameObject, box);
            }
        }
    }
    
    // The cooldown duration in seconds
    [SerializeField] private float dropOffCooldown = 10f;
    
    // Tracks when the player is allowed to use the drop-off again
    private float nextDropOffTime = 0f;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DropOffZone"))
        {
            //So we cant spam/holddown the key
            if (Time.time >= nextDropOffTime && Keyboard.current.eKey.wasPressedThisFrame)
            {
                RoundManager.Instance.NextRound();
                
                nextDropOffTime = Time.time + dropOffCooldown;
            }
        }
    }

    private void PickUpObject(GameObject collideBoxObject, BoxCollision boxCollision)
    {
        playerHasPickedUpObject = true;
        pickedUpObject = collideBoxObject;
        currentBoxData = boxCollision; 

        Rigidbody boxRb = pickedUpObject.GetComponent<Rigidbody>();
        if (boxRb != null) boxRb.isKinematic = true; 

        Collider boxCollider = pickedUpObject.GetComponent<Collider>();
        if (boxCollider != null) boxCollider.enabled = true;

        // Snap to hand
        collideBoxObject.transform.SetParent(playerHand);
        
        collideBoxObject.transform.localPosition = Vector3.zero;
        
        //When we pick up an object that is rotated it causes the object to be picked up and freeze at the angle. this line was not the problem.
        collideBoxObject.transform.localRotation = Quaternion.identity;

        playerStealthController.moveSpeed -= boxCollision.boxWeight;
    }

    private void DropObject()
    {
        pickedUpObject.transform.SetParent(null);

        BoxCollider boxCollider = pickedUpObject.GetComponent<BoxCollider>();
        if (boxCollider != null) boxCollider.enabled = true;

        Rigidbody boxRb = pickedUpObject.GetComponent<Rigidbody>();
        if (boxRb != null) 
        {
            boxRb.isKinematic = false; 
            
            Vector3 throwDirection = transform.forward + (Vector3.up * 0.5f);
            
            boxRb.linearVelocity = throwDirection.normalized * throwForce;
            
            boxRb.useGravity = true;
        }

        if (currentBoxData != null)
        {
            playerStealthController.moveSpeed += currentBoxData.boxWeight;
        }

        playerHasPickedUpObject = false;
        pickedUpObject = null;
        currentBoxData = null;
    }
}