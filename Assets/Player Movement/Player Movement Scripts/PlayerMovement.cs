using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;

    [Header("Core Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    
    [Header("Slope Slide Settings")]
    [Tooltip("How much the slope angle multiplies your speed")]
    [SerializeField] private float slopeSpeedMultiplier = 0.5f; 
    [Tooltip("Minimum speed so you don't instantly stop on flat ground")]
    [SerializeField] private float baseSlideSpeed = 8f;
    private bool isSliding;

    [Header("Wall Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector3 wallJumpingPower = new Vector3(8f, 16f, 0f);
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    [Header("Physics & Collision")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [Tooltip("Higher = falls faster")]
    [SerializeField] private float gravityScale = 3f; 

    private void Update()
    {
        horizontal = -Input.GetAxisRaw("Horizontal");
        
        isSliding = Input.GetKey(KeyCode.LeftControl) && IsGrounded();

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpingPower, rb.linearVelocity.z);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        WallSlide();
        WallJump();

        if (!isWallJumping && !isSliding) 
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!isWallSliding && !isSliding)
        {
            Vector3 gravity = Physics.gravity.y * (gravityScale - 1) * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        //can only do 1 of these at a time
        if (isSliding)
        {
            ApplySlopeSlide();
        }
        else if (!isWallJumping)
        {
            rb.linearVelocity = new Vector3(horizontal * speed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    //Youtube reference: https://www.youtube.com/watch?v=SsckrYYxcuM&t=138s
    private void ApplySlopeSlide()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, 1f, groundLayer))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            
            Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;

            if (slopeAngle < 2f)
            {
                slopeDirection = new Vector3(isFacingRight ? 1f : -1f, 0f, 0f);
            }

            float currentSlideSpeed = baseSlideSpeed + (slopeAngle * slopeSpeedMultiplier);

            rb.linearVelocity = new Vector3(slopeDirection.x * currentSlideSpeed, slopeDirection.y * currentSlideSpeed, rb.linearVelocity.z);
        }
    }

    //Ground and wall detection
    private bool IsGrounded() => Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    private bool IsWalled() => Physics.CheckSphere(wallCheck.position, 0.2f, wallLayer);

    //Wall logic
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlidingSpeed), rb.linearVelocity.z);
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

    private void StopWallJumping() => isWallJumping = false;

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