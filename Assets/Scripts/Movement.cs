using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [HideInInspector] public bool isPausedFromMenu = false;

    [Header("Ustawienia Ruchu")]
    [SerializeField] private float walkingSpeed = 8f;
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float jumpHeight = 2.5f; 
    [SerializeField] private float gravity = -45f;      

    [Header("Ustawienia Drabiny")]
    [SerializeField] private float ladderSpeed = 5f;
    private bool isOnLadder = false;

    [Header("Ustawienia Kucania")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 10f;
    [SerializeField] private float crouchSpeed = 4f;

    [Header("Detekcja Podłoża")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundedDistance = 0.5f;

    [Header("Kamera")]
    public Camera playerCamera;
    [SerializeField] private Transform camHolder;
    public float lookSpeed = 2.0f; 
    [SerializeField] private float lookXLimit = 85.0f; 

    private CharacterController characterController;
    private float rotationX = 0;
    private Vector3 velocity;
    private bool isGrounded;

    public override void OnStartLocalPlayer()
    {
        characterController = GetComponent<CharacterController>();
        characterController.height = standingHeight;
        characterController.center = new Vector3(0, standingHeight / 2, 0);
        isPausedFromMenu = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer && playerCamera != null)
            playerCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Jeśli pauza, uwalniamy kursor i blokujemy resztę skryptu
        if (isPausedFromMenu) 
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }

        // Jeśli nie ma pauzy, pilnujemy blokady kursora
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 1. Detekcja ziemi
        isGrounded = characterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundedDistance, groundMask);

        if (isGrounded && velocity.y < 0 && !isOnLadder)
        {
            velocity.y = -2f; 
        }

        // 2. Kamera
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        if (camHolder != null) camHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Ruch
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (isOnLadder)
        {
            // RUCH NA DRABINIE (Vertical porusza w górę/dół)
            Vector3 ladderMove = new Vector3(moveX * walkingSpeed * 0.5f, moveZ * ladderSpeed, 0);
            // Przeliczamy na kierunek lokalny gracza
            ladderMove = transform.TransformDirection(ladderMove);
            
            characterController.Move(ladderMove * Time.deltaTime);
            velocity.y = 0; // Wyłączamy grawitację na drabinie

            // Zeskakiwanie z drabiny
            if (Input.GetButtonDown("Jump"))
            {
                isOnLadder = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // STANDARDOWY RUCH
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

    // --- TRIGGERY DRABINY ---
    private void OnTriggerStay(Collider other)
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