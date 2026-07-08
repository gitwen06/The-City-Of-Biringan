using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Look")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerFlashlight;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    private float pitch = 0f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    private CapsuleCollider capsule;
    private bool isCrouching = false;

    private InputSystem_Actions inputActions;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Crouch.performed += OnCrouchPerformed;
        inputActions.Player.Crouch.canceled += OnCrouchCanceled;
        inputActions.Player.Flashlight.performed += OnFlashlightPerformed;
        inputActions.Player.Jump.performed += OnJumpPerformed;
    }


    private void OnDisable()
    {
        inputActions.Player.Crouch.performed -= OnCrouchPerformed;
        inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        inputActions.Player.Flashlight.performed -= OnFlashlightPerformed;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        // Horizontal look — rotate the whole player left/right
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        // Vertical look — rotate only the camera up/down, clamped
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        playerFlashlight.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void FixedUpdate()
    {
        bool isRunning = inputActions.Player.Sprint.IsPressed();
        float currentSpeed = isRunning ? moveSpeed * 2.0f : moveSpeed;


        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, capsule.bounds.extents.y + 0.1f);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        }
    }

    private void OnFlashlightPerformed(InputAction.CallbackContext context)
    {
        playerFlashlight.gameObject.SetActive(!playerFlashlight.gameObject.activeSelf);
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        isCrouching = true;
        capsule.height = crouchHeight;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        isCrouching = false;
        capsule.height = standHeight;
    }
}