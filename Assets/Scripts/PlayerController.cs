using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Ustawienia Ruchu")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float sprintSpeed = 11f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -25f;

    [Header("Ustawienia Kamery")]
    public Transform camHolder;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookXLimit = 85f;
    private float xRotation = 0f;

    [Header("Fizyka i Podłoże")]
    public Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Kucanie")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchSmoothness = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Jeśli to nie jest nasz lokalny gracz, wyłączamy mu kamerę i skrypt
        if (!isLocalPlayer)
        {
            if (camHolder != null)
            {
                Camera cam = camHolder.GetComponentInChildren<Camera>();
                if (cam != null) cam.enabled = false;
                
                AudioListener al = camHolder.GetComponentInChildren<AudioListener>();
                if (al != null) al.enabled = false;
            }
            return;
        }

        UpdateCursorState(true);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Obsługa myszki (ESC odblokowuje kursor)
        if (Input.GetKeyDown(KeyCode.Escape)) UpdateCursorState(false);
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked) UpdateCursorState(true);

        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Logika poruszania
        CheckGround();
        HandleRotation();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        
        // Aplikacja grawitacji
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);

        camHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = sprintSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleCrouch()
    {
        float targetHeight = Input.GetKey(KeyCode.LeftControl) ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSmoothness);
    }

    private void UpdateCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}