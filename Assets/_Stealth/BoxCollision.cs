using System;
using UnityEngine;

public class BoxCollision : MonoBehaviour
{
    public float boxWeight;
    public float boxWorthAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropOffZone"))
        {
            other.GetComponent<UpdateQuota>().totalQuota += boxWorthAmount;
        }
    }
}
