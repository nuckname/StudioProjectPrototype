using System;
using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    //use transforms later
    
    private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    
    [SerializeField] private Vector3 playerTpLocation;
    [SerializeField] private Vector3 cameraTpLocation;

    private void Start()
    {
        //characterController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //move player
            characterController.enabled = false;
            characterController.transform.position = playerTpLocation;
            characterController.enabled = true;
            
            //move camera
            playerCamera.transform.position = cameraTpLocation;
        }
    }
}