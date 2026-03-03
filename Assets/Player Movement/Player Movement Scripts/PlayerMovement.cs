using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    
    [Header("Movement Value Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    
    private bool isFacingRight = true;
    
    [Header("Wall Slide Values")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;

    [Header("Wall Jump Values")]
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector3 wallJumpingPower = new Vector3(8f, 16f, 0f);
    private float wallJumpingCounter;

    [Header("Stuff")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private void Update()
    {
        //They are reverse for some reason, so I added negative 
        horizontal = -Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpingPower, rb.linearVelocity.z);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector3(horizontal * speed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics.CheckSphere(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue), rb.linearVelocity.z);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            
            wallJumpingDirection = isFacingRight ? -1f : 1f; 
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector3(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y, rb.linearVelocity.z);
            wallJumpingCounter = 0f;

            float currentFacingDir = isFacingRight ? 1f : -1f;
            if (currentFacingDir != wallJumpingDirection)
            {
                ExecuteFlip();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            ExecuteFlip();
        }
    }

    private void ExecuteFlip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
