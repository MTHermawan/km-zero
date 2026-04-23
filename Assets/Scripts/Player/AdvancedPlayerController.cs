
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AdvancedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public float gravity = -19.62f;
    private bool _canMove = true;
    public bool canMove {
        get => _canMove; 
        private set
        {
            if (_canMove != value)
            {
                _canMove = value;
                Cursor.lockState = _canMove ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }
    }

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.5f;
    private Transform cameraTransform;
    public Transform bodyTransform;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private Vector2 moveInput;
    private Vector2 lookInput;

    // BOOLEAN HOLD STATE
    private bool isSprintHeld;
    private bool isCrouchHeld;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove)
        {
            isSprintHeld = InputSystem.actions.FindAction("Sprint").IsPressed();
            isCrouchHeld = InputSystem.actions.FindAction("Crouch").IsPressed();
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0) velocity.y = -2f;

            // 2. SPEED LOGIC (PRIORITY: CROUCH > RUN > WALK)
            float currentSpeed = walkSpeed;
            if (isCrouchHeld)
            {
                currentSpeed = crouchSpeed;
                transform.localScale = new Vector3(1, 0.6f, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1f, 1);
                if (isSprintHeld)
                {
                    currentSpeed = runSpeed;
                }
            }

            // 3. MOVE
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        // 4. GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded && !isCrouchHeld)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void OnInteract(InputValue value)
    {
        // Debug.Log("Trying to interact.");
        if (TryGetComponent(out Interactor interactable))
        {
            interactable.PerformInteract();
        }
    }

    public void EnableCamera() => cameraTransform.gameObject.SetActive(true);
    public void DisableCamera() => cameraTransform.gameObject.SetActive(false);

    public void EnableMovement() => canMove = true;
    public void DisableMovement() => canMove = false;

    private static AdvancedPlayerController s_instance;
    public static AdvancedPlayerController Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<AdvancedPlayerController>();
            }
            return s_instance;
        }
    }
}