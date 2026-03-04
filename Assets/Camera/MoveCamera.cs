using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private float downwardSpeed = 2f;

    void Update()
    {
        transform.position += Vector3.down * (downwardSpeed * Time.deltaTime);
    }
}