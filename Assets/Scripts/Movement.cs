using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 8f;
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float jumpHeight = 2.5f; 
    [SerializeField] private float gravity = -45f;      // Mocna grawitacja

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
        
        // Naprawa środka kapsuły na starcie
        characterController.height = standingHeight;
        characterController.center = new Vector3(0, standingHeight / 2, 0);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // 1. SPRAWDZANIE ZIEMI (Bardziej rygorystyczne)
        // Raycast strzela z dołu kapsuły w dół
        isGrounded = characterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundedDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            // Lekki docisk do ziemi, żeby isGrounded zawsze wyłapywało kontakt
            velocity.y = -5f; 
        }

        // 2. OBRÓT
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        camHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 3. RUCH I SKOK
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Kucanie
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * timeToCrouch);
        characterController.center = new Vector3(0, characterController.height / 2, 0);

        // Chodzenie
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed);

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Skok
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // sqrt(wysokość * -2 * grawitacja)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // GRAWITACJA (Zastosowana dwa razy dla szybkości spadania)
        velocity.y += gravity * Time.deltaTime;
        
        // Przesunięcie pionowe
        characterController.Move(velocity * Time.deltaTime);
    }
}