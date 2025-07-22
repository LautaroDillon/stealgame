using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaibor : MonoBehaviour
{
    #region Variables
    [Header("player movement settings")]
    public float walkSpeed;
    public float crouchSpeed;

    public float groundDrag;
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

    [Header("hearing settings")]
    public float nRadius;
    public float crouchRadius;
    public float runingRadius;
    public float jumpRadius;
    public LayerMask enemysMasck;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents the player from tipping over
        Invoke(nameof(resetJup), jumpCooldown);

    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHight * 0.5f + 0.2f, whatisground);

        MyInputs();
        SpeedControl();

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
    }

    private void movePlayer()
    {
        moveDirection = orientation.forward * VerticalInput + orientation.right * HorizontalInput;
        //on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * walkSpeed * 10, ForceMode.Force);
        //in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * walkSpeed * 10 * airMultiplier, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > walkSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * walkSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset y velocity

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void resetJup()
    {
        readyToJump = true;
    }

}