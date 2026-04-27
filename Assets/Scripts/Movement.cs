using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 8f;
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float jumpHeight = 2.5f; 
    [SerializeField] private float gravity = -45f;      

    [Header("Ladder Settings")]
    [SerializeField] private float ladderSpeed = 5f;
    private bool isOnLadder = false;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 10f;
    [SerializeField] private float crouchSpeed = 4f;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundedDistance = 0.5f;

    [Header("Camera")]
    public Camera playerCamera;
    [SerializeField] private Transform camHolder;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 85.0f; 

    private CharacterController characterController;
    private float rotationX = 0;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        if (!isLocalPlayer)
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        
        characterController.height = standingHeight;
        characterController.center = new Vector3(0, standingHeight / 2, 0);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        isGrounded = characterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundedDistance, groundMask);

        // Reset grawitacji tylko jeśli nie jesteśmy na drabinie
        if (isGrounded && velocity.y < 0 && !isOnLadder)
        {
            velocity.y = -2f; 
        }

        // Obrót kamery
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        camHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (isOnLadder)
        {
            // RUCH NA DRABINIE
            // Poruszamy się góra/dół za pomocą W/S
            Vector3 ladderMove = new Vector3(0, moveZ * ladderSpeed, 0);
            characterController.Move(ladderMove * Time.deltaTime);
            
            // Zerujemy grawitację, żeby nie spadać podczas wspinaczki
            velocity.y = 0;

            // Opcjonalnie: skok pozwala "odbić się" od drabiny
            if (Input.GetButtonDown("Jump"))
            {
                isOnLadder = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // NORMALNY RUCH
            bool isCrouching = Input.GetKey(KeyCode.LeftControl);
            float targetHeight = isCrouching ? crouchHeight : standingHeight;
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * timeToCrouch);
            characterController.center = new Vector3(0, characterController.height / 2, 0);

            float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed);

            Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;
            characterController.Move(move * currentSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    // Wykrywanie drabiny
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }
}