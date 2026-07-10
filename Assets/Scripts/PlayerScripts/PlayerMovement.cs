using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Look")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerFlashlight;
    [SerializeField] private Slider playerHealth;
    [SerializeField] private Slider playerStamina;
    [SerializeField] private float cameraFOVChangeRate = 1.25f;
    [SerializeField] private float staminaDrainRate = 15.5f;
    [SerializeField] private float staminaRegenRate = 12.5f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    private float pitch = 0f;

    [Header("Stamina")]
    [Tooltip("Maximum stamina value shown in the UI slider")]
    [SerializeField] private float maxStamina = 100f;
    [Tooltip("Seconds to wait after sprinting before stamina begins to regenerate")]
    [SerializeField] private float staminaRegenDelay = 1.2f;
    [Tooltip("Minimum stamina required to resume sprinting after exhaustion")]
    [SerializeField] private float exhaustionRecoveryThreshold = 15f;
    private float currentStamina;
    private bool isExhausted = false;
    private float regenDelayTimer = 0f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    private CapsuleCollider capsule;
    private bool isCrouching = false;
    private bool isRunning = false;



    private InputSystem_Actions inputActions;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 move;


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

        // initialize stamina
        currentStamina = maxStamina;
        if (playerStamina != null)
        {
            playerStamina.value = currentStamina;
        }
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
        // determine desired sprint input
        bool wantsToSprint = inputActions.Player.Sprint.IsPressed();

        // disable sprinting while exhausted
        isRunning = wantsToSprint && !isExhausted && moveInput != Vector2.zero;

        float currentSpeed = isRunning ? moveSpeed * 2.0f : moveSpeed;

        // handle stamina drain when actually sprinting (and moving)
        if (isRunning)
        {
            // reset regen delay while sprinting
            regenDelayTimer = staminaRegenDelay;

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 75f, cameraFOVChangeRate * Time.fixedDeltaTime);
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
            if (currentStamina <= 0f)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60f, cameraFOVChangeRate * Time.fixedDeltaTime);
                currentStamina = 0f;
                isExhausted = true;
                // give a slightly longer delay after exhaustion
                regenDelayTimer = staminaRegenDelay + 0.5f;
            }
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60f, cameraFOVChangeRate * Time.fixedDeltaTime);
            // not sprinting: count down delay then regenerate
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= Time.fixedDeltaTime;
            }
            else
            {
                currentStamina += staminaRegenRate * Time.fixedDeltaTime;
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                }

                // recover from exhaustion when we have enough stamina
                if (isExhausted && currentStamina >= exhaustionRecoveryThreshold)
                {
                    isExhausted = false;
                }
            }
        }

        // clamp and apply to UI
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        if (playerStamina != null)
        {
            playerStamina.value = currentStamina;
        }

        // move the player
        move = (transform.right * moveInput.x + transform.forward * moveInput.y) * currentSpeed * Time.fixedDeltaTime;
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
            rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
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