using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded = true;

    public float moveSpeed;
    public float jumpForce;

    public PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        // Assign different stats based on player index
        if (playerInput.playerIndex == 0) // Player 1
        {
            moveSpeed = 10f;
            jumpForce = 5f;
        }
        else if (playerInput.playerIndex == 1) // Player 2
        {
            moveSpeed = 5f;
            jumpForce = 10f;
        }

        // Subscribe to Input System events
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => Jump();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
