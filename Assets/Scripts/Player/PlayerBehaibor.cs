using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaibor : MonoBehaviour
{
    #region Variables
    [Header("player movement settings")]
    private float Speed;
    public float walkSpeed;
    public float crouchSpeed;
    public float runSpeed;
    bool isRuning;

    public float groundDrag;
    [Header("crouch settings")]
    public float crouchYScale;
    float StartYScale;

    [Header("Jump settings")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Ground check settings")]
    public float playerHight;
    public LayerMask whatisground;
    public bool grounded;

    public Transform orientation;

    [Header("player settings")]
    float HorizontalInput;
    float VerticalInput;
    Vector3 moveDirection;
    public Rigidbody rb;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("hearing settings")]
    public float nRadius;
    public float crouchRadius;
    public float runingRadius;
    public float jumpRadius;
    public LayerMask enemysMasck;

    [Header("Slope settings")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    public Throwing throwing;

    public MovementType state;
    public enum MovementType
    {
        walking,
        running,
        crouching,
        air
    }
    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents the player from tipping over
        Invoke(nameof(resetJup), jumpCooldown);

        StartYScale = transform.localScale.y; // Store the initial Y scale of the player
    }

    void Update()
    {
        //check if te player is on he ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHight * 0.5f + 0.2f, whatisground);

        MyInputs();
        SpeedControl();
        Statehandler();
        //poner cuando el player cae en el suelo despues de saltar
        HearingManager.instance.OnSoundoEmited(transform.position, EHearingSensosType.Ejump, 0.25f);


        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        //if (IsWalking)
        {
            movePlayer();
        }
    }

    public void MyInputs()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJup), jumpCooldown);
        }

        if (Input.GetKey(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, StartYScale, transform.localScale.z);

        }
    }

    void Statehandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementType.crouching;
            Speed = crouchSpeed;
        }
        //setting the speed based on the state
        if (grounded && Input.GetKey(runKey))
        {
            state = MovementType.running;
            isRuning = true;
            Speed = runSpeed;
        }
        else if (grounded)
        {
            state = MovementType.walking;
            Speed = walkSpeed;
        }
        else if (!grounded)
        {
            state = MovementType.air;
            Speed = walkSpeed * airMultiplier; // Air speed is affected by air multiplier
        }
    }

    private void movePlayer()
    {
        moveDirection = orientation.forward * VerticalInput + orientation.right * HorizontalInput;

        if (onSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopDirection() * Speed * 20, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80, ForceMode.Force); // Prevents the player from jumping off slopes
            }
        }

        //on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * Speed * 10, ForceMode.Force);
        //in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * Speed * 10 * airMultiplier, ForceMode.Force);

        HearingManager.instance.OnSoundoEmited(transform.position, EHearingSensosType.Efootstep, isRuning ? 0.2f : 0.1f);

        rb.useGravity = !onSlope(); // Disable gravity when on slope to prevent sliding down
    }

    void SpeedControl()
    {
        if (onSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > Speed)
                rb.velocity = rb.velocity.normalized * Speed;
        }

        // limit velociy
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > Speed)
            {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;

        // Reset the y velocity to prevent stacking forces
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset y velocity
        // apply the jmp foce
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void resetJup()
    {
        readyToJump = true;
        exitingSlope = false;
    }


    bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var gam = collision.gameObject;
        if (gam.layer == 31)
        {
            if (gam.tag == "Throw")
            {

                throwing.addToThrows(1); // Add the projectile to the list of throwable items
                gam.SetActive(false); // Disable the game object instead of destroying it
            } 
        }
    }
}