using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    private PlayerUIController PlayerUI => PlayerUIController.Instance;
    public PlayerInputController Input { get; private set; }

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public float gravity = -19.62f;
    public HashSet<string> _disableMoveSet = new();

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.5f;
    public Transform CameraTransform { get; private set; }
    public Transform bodyTransform;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private int _unlockCursorCount = 0;
    public int UnlockCursorCount
    {
        get => _unlockCursorCount;
        private set
        {
            _unlockCursorCount = Mathf.Clamp(value, 0, value);
            if (UnlockCursorCount > 0)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    // BOOLEAN HOLD STATE
    private bool isSprintHeld;
    private bool isCrouchHeld;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Input = GetComponent<PlayerInputController>();
        CameraTransform = Camera.main.transform;
    }

    void Start()
    {
        InitMovement();
    }

    void Update()
    {
        if (CanMove())
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
            Vector3 move = transform.right * Input.MoveInputVector.x + transform.forward * Input.MoveInputVector.y;
            controller.Move(currentSpeed * Time.deltaTime * move);
        }

        // 4. GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void InitMovement()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Input.onLook += Look;
        Input.onJump += Jump;
    }

    public void Look()
    {
        if (!CanMove()) return;

        float mouseX = Input.LookInputVector.x * mouseSensitivity;
        float mouseY = Input.LookInputVector.y * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Jump()
    {
        if (!CanMove() || !isGrounded || isCrouchHeld) return;

        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    public void EnableCamera()
    {
        // PlayerUI.EnableCrosshair();
        CameraTransform.gameObject.SetActive(true);
    }
    public void DisableCamera()
    {
        // PlayerUI.DisableCrosshair();
        CameraTransform.gameObject.SetActive(false);
    }

    public void DisableMovement(string disableId) => _disableMoveSet.Add(disableId);
    public void EnableMovement(string disableId) => _disableMoveSet.Remove(disableId);
    private bool CanMove() => _disableMoveSet.Count <= 0;
    public void LockCursor() => UnlockCursorCount--;
    public void UnlockCursor() => UnlockCursorCount++;

    private static PlayerController s_instance;
    public static PlayerController Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<PlayerController>();
            }
            return s_instance;
        }
    }
}