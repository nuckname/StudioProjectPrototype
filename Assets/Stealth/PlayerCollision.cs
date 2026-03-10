using System;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerStealthController playerStealthController;

    [SerializeField] private Transform playerHand; 
    [SerializeField] private bool playerHasPickedUpObject;

    private void Awake()
    {
        playerStealthController = GetComponent<PlayerStealthController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box") && !playerHasPickedUpObject)
        {
            BoxCollision box = other.gameObject.GetComponent<BoxCollision>();
            if (box != null)
            {
                PickUpObject(other.gameObject, box);
            }
        }
    }

    private void PickUpObject(GameObject boxObject, BoxCollision boxData)
    {
        playerHasPickedUpObject = true;

        boxObject.transform.SetParent(playerHand);

        playerStealthController.moveSpeed -= boxData.boxWeight;
    }
}