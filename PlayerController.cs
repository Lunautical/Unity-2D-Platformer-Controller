using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// ---LUNAUTICAL'S CODE--- \\\
/// Thanks for using my script
/// You are free to use and change the script as you wish
/// If you do showcase the script in a video, all I ask of you is to promote the video the script came from (Unity How To #3)
/// Hope you enjoy!
/// </summary>


public class PlayerController : MonoBehaviour
{
    //Assignables
    public Rigidbody2D rb2d;
    public BoxCollider2D boxCollider2D;

    public LayerMask groundLayerMask;

    //Values
    public float speed = 500f;
    public float maxSpeed = 5f;
    public float jumpStrength = 8f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;

    float groundDistance = 0.2f;

    [SerializeField] private float xInput;
    [SerializeField] private Vector2 vel;

    public float dashDistance = 200f;
    bool isDashing = false;
    public float maxNumberOfDashes = 1f;
    [SerializeField] private float currentNumberOfDashes;

    public float maxNumberOfJumps = 1f;
    [SerializeField] private float currentNumberOfJumps;

    public float wallDistance = .2f;
    public float wallSlideSpeed = 3f;
    public float wallJumpDistance = 8f;
    public float wallJumpHeight = 4f;

    //Bools
    [SerializeField] private bool isGrounded;

    private bool isFacingRight = true;
    private float facingDirection;

    [SerializeField] private bool wallCheckHit;
    [SerializeField] private bool isWallSliding;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        PlayerInput();
        GroundCheck();
        FacingOrientation();
        WallSlide();

        //Extras

        //Checking if sliding on wall
        if (wallCheckHit)
            isWallSliding = true;
        else
            isWallSliding = false;
    }

    private void FixedUpdate()
    {
        vel = rb2d.velocity;
        Movement();
    }

    void PlayerInput()
    {
        //Player's left and right input
        if (Input.GetKey(KeyCode.D))
        {
            isFacingRight = true;
            xInput = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            isFacingRight = false;
            xInput = -1;
        }
        else
        {
            xInput = 0;
        }

        //For Jumping
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!isDashing && isWallSliding && !isGrounded)
                WallJump();
            else
            if(!isDashing)
                Jump();
        }

        //Dash Checking Left
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && currentNumberOfDashes > 0 && xInput < 0)
        {
            StartCoroutine(Dash(-1f));
        }

        //Dash Checking Left
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && currentNumberOfDashes > 0 && xInput > 0)
        {
            StartCoroutine(Dash(1f));
        }
    }

    void Movement()
    {
        //Cancelling input if speed is over max speed
        if (xInput > 0 && vel.x > maxSpeed)
        {
            xInput = 0;
        }else
        if (xInput < 0 && vel.x < -maxSpeed)
        {
            xInput = 0;
        }

        //Applying forces
        if(!isDashing)
            rb2d.AddForce(transform.right * xInput * speed * Time.deltaTime);

        //Counter Movement
        if (Math.Abs(vel.x) > threshold && Math.Abs(xInput) < 0.05f && !isDashing || (vel.x < -threshold && xInput > 0) || (vel.x > threshold && xInput < 0))
        {
            rb2d.AddForce(speed * transform.right * Time.deltaTime * -vel.x * counterMovement);
        }

        //Wall Sliding
        if (isWallSliding && xInput > 0 || isWallSliding && xInput < 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, wallSlideSpeed, float.MaxValue));
        }
    }

    void Jump()
    {
        if(isGrounded)
        {
            vel.y = jumpStrength;
            rb2d.velocity = vel;
        }
        else if (!isGrounded && currentNumberOfJumps > 0)
        {
            currentNumberOfJumps--;
            vel.y = jumpStrength;
            rb2d.velocity = vel;
        }
    }

    void WallJump()
    {
        if (isFacingRight)
            facingDirection = 1;
        else
            facingDirection = -1;
        rb2d.velocity = new Vector2(wallJumpDistance * -facingDirection, wallJumpHeight);
    }

    void FacingOrientation()
    {
        if (isFacingRight && !isDashing)
            transform.localScale = new Vector2(1, transform.localScale.y);

        if (!isFacingRight && !isDashing)
            transform.localScale = new Vector2(-1, transform.localScale.y);
    }

    void WallSlide()
    {
        if (isFacingRight)
        {
            wallCheckHit = Physics2D.Raycast(transform.position, new Vector2(wallDistance, 0), wallDistance, groundLayerMask);
            Debug.DrawRay(transform.position, new Vector2(wallDistance, 0), Color.blue);
        } else
        {
            wallCheckHit = Physics2D.Raycast(transform.position, new Vector2(-wallDistance, 0), wallDistance, groundLayerMask);
            Debug.DrawRay(transform.position, new Vector2(-wallDistance, 0), Color.blue);
        }
    }

    void GroundCheck()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, groundDistance, groundLayerMask);
        Color rayColor;
        if(raycastHit.collider != null)
        {
            isGrounded = true;
            currentNumberOfDashes = maxNumberOfDashes;
            currentNumberOfJumps = maxNumberOfJumps;
            rayColor = Color.green;
        }
        else
        {
            isGrounded = false;
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center, Vector2.down * (boxCollider2D.bounds.extents.y + groundDistance), rayColor);
    }

    IEnumerator Dash(float direction)
    {
        Debug.Log("Is Dashing");
        isDashing = true;
        currentNumberOfDashes--;
        rb2d.velocity = new Vector2(dashDistance * direction, 0f);
        float gravity = rb2d.gravityScale;
        rb2d.gravityScale = 0;
        yield return new WaitForSeconds(0.4f);
        isDashing = false;
        rb2d.gravityScale = gravity;
    }
}
