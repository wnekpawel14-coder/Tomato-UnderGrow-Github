using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [HideInInspector] public bool isPausedFromMenu = false;

    [Header("Kamera")]
    public Camera playerCamera;
    [SerializeField] private Transform camHolder;
    [SerializeField] private float lookSpeed = 2.0f; 
    [SerializeField] private float lookXLimit = 85.0f; 

    [Header("Ruch")]
    [SerializeField] private float walkingSpeed = 8f;
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float jumpHeight = 2.5f; 
    [SerializeField] private float gravity = -45f;

    private CharacterController characterController;
    private float rotationX = 0;
    private Vector3 velocity;
    private bool isGrounded;

    public override void OnStartLocalPlayer()
    {
        characterController = GetComponent<CharacterController>();
        isPausedFromMenu = false;
        // Na starcie lokujemy kursor raz
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Jeśli menu jest aktywne, całkowicie ignorujemy Update
        if (isPausedFromMenu) return;

        // 1. Logika ziemi
        isGrounded = characterController.isGrounded || Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.5f);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 2. Obrót kamery
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        if (camHolder != null) camHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Chodzenie
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;
        characterController.Move(move * walkingSpeed * Time.deltaTime);

        // 4. Skok i grawitacja
        if (Input.GetButtonDown("Jump") && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}