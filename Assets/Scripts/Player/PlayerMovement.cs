using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public bool isMovementDisabled = false; // Flag to control movement

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float JumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        
    }

    // void Update()
    // {

    //     isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    //     if(isGrounded && velocity.y < 0) {
    //         velocity.y = -2f;
    //     }

    //     // if (isMovementDisabled)
    //     // {
    //     //     return; // Skip movement if movement is disabled
    //     // }

    //     float x = Input.GetAxis("Horizontal");
    //     float z = Input.GetAxis("Vertical");

    //     Vector3 move = transform.right * x + transform.forward * z;

    //     controller.Move(move * speed * Time.deltaTime);

    //     if(Input.GetButtonDown("Jump") && isGrounded) {
    //         velocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
    //     }

    //     velocity.y += gravity * Time.deltaTime;

    //     controller.Move(velocity * Time.deltaTime);
    // }

    void Update()
    {
        // Check if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Allow disabling movement while keeping the CharacterController operational
        if (!isMovementDisabled)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
            }
        }

        // Gravity always affects the player
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); // Ensure the controller stays active
    }
}
