using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrain = 20f;
    public float staminaRegen = 15f;
    private float currentStamina;
    private bool canSprint = true; // HARD stamina flag

    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("Crosshair Settings")]
    public Image defaultCrosshair;
    public Image interactCrosshair;

    [Header("UI Settings")]
    public Slider staminaSlider;
    public TMP_Text staminaText;

    [Header("Pickup Settings")]
    public Transform holdPosition;
    public GameObject heldObject;
    private Vector3 originalScale;
    public float throwForce = 10f;

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    public AudioClip throwSound;
    private AudioSource audioSource;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float crouchSpeed = 2.5f;
    private float standHeight;
    private Vector3 standCenter;
    private Vector3 standingCameraLocalPos;
    private Vector3 crouchingCameraLocalPos;
    private bool isCrouching = false;

    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        audioSource = GetComponent<AudioSource>();

        standHeight = controller.height;
        standCenter = controller.center;
        standingCameraLocalPos = playerCamera.transform.localPosition;
        crouchingCameraLocalPos = new Vector3(
            standingCameraLocalPos.x,
            standingCameraLocalPos.y - (standHeight - crouchHeight) / 2f - 0.3f,
            standingCameraLocalPos.z);
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        currentStamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;

        SetupUI();

        if (holdPosition == null)
        {
            GameObject holdPos = new GameObject("HoldPosition");
            holdPos.transform.SetParent(playerCamera.transform);
            holdPos.transform.localPosition = new Vector3(0.5f, -0.3f, 1f);
            holdPosition = holdPos.transform;
        }

        if (defaultCrosshair != null) defaultCrosshair.enabled = true;
        if (interactCrosshair != null) interactCrosshair.enabled = false;
    }

    void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleCrouch();
        HandlePickup();
        HandleStamina();
        CheckForPickableObjects();
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Determine sprint (disabled while crouching or in the air)
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && canSprint && move.magnitude > 0.1f && !isCrouching && isGrounded;
        float speed = isCrouching ? crouchSpeed : (wantsToSprint ? sprintSpeed : walkSpeed);

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Drain stamina if sprinting
        if (wantsToSprint)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canSprint = false; // stop sprinting until full
            }
        }
    }

    void HandleCrouch()
    {
        bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (ctrlHeld && !isCrouching)
        {
            isCrouching = true;
            controller.height = crouchHeight;
            // Keep capsule bottom at same floor level
            float crouchCenterY = (standCenter.y - standHeight / 2f) + crouchHeight / 2f;
            controller.center = new Vector3(standCenter.x, crouchCenterY, standCenter.z);
            playerCamera.transform.localPosition = crouchingCameraLocalPos;
        }
        else if (!ctrlHeld && isCrouching)
        {
            // Raycast from just above the crouched capsule top to check for ceiling
            Vector3 rayOrigin = transform.position + Vector3.up * (crouchHeight + 0.05f);
            float checkDistance = standHeight - crouchHeight - 0.05f;
            if (checkDistance <= 0f || !Physics.Raycast(rayOrigin, Vector3.up, checkDistance))
            {
                isCrouching = false;
                controller.height = standHeight;
                controller.center = standCenter;
                playerCamera.transform.localPosition = standingCameraLocalPos;
            }
        }
    }

    void HandlePickup()
    {
        // ===== E — pouze sebrání / interakce =====
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position,
                                playerCamera.transform.forward,
                                out hit,
                                interactDistance,
                                interactLayer))
            {
                //  Dveøe
                InteractableDoor hitDoor = hit.collider.GetComponent<InteractableDoor>();
                if (hitDoor != null)
                {
                    hitDoor.Interact();
                    return;
                }

                //  Truhla
                ChestUse hitChest = hit.collider.GetComponent<ChestUse>();
                if (hitChest != null)
                {
                    hitChest.Interact();
                    return;
                }

                //  Pickable objekt
                if (hit.collider.CompareTag("Pickable"))
                {
                    heldObject = hit.collider.gameObject;

                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;

                    originalScale = heldObject.transform.localScale;
                    heldObject.transform.localScale = originalScale * 0.7f;

                    heldObject.transform.SetParent(holdPosition);
                    heldObject.transform.localPosition = new Vector3(0f, -0.1f, -0.1f);
                    heldObject.transform.localRotation = Quaternion.Euler(0f, 230f, 0f);

                    if (pickupSound && audioSource)
                        audioSource.PlayOneShot(pickupSound);

                    return;
                }

                //  Ostatní interakce
                if (hit.collider.CompareTag("Interactable"))
                {
                    Animator animator = hit.collider.GetComponent<Animator>();
                    if (animator != null)
                        animator.SetTrigger("Interact");
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && heldObject != null)
        {
            DropObject();
        }

        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();

            if (throwSound && audioSource)
                audioSource.PlayOneShot(throwSound);

            DropObject();

            if (rb != null)
                rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        }
    }
    void DropObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            heldObject.transform.localScale = originalScale;
            heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }

    void HandleStamina()
    {
        // Regen only if not sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && controller.velocity.magnitude > 0.1f && canSprint;

        if (!isSprinting)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                canSprint = true; 
            }
        }

        // Update slider
        if (staminaSlider != null)
        {
            if (isSprinting || currentStamina < maxStamina)
            {
                if (!staminaSlider.gameObject.activeSelf)
                    staminaSlider.gameObject.SetActive(true);
                staminaSlider.value = currentStamina / maxStamina;
            }
            else staminaSlider.gameObject.SetActive(false);
        }

        if (staminaText != null)
            staminaText.text = Mathf.RoundToInt(currentStamina).ToString();
    }

    void CheckForPickableObjects()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.GetComponent<InteractableDoor>() != null || hit.collider.CompareTag("Pickable") || hit.collider.CompareTag("Interactable"))
            {
                if (defaultCrosshair != null) defaultCrosshair.enabled = false;
                if (interactCrosshair != null) interactCrosshair.enabled = true;
                return;
            }
        }

        if (defaultCrosshair != null) defaultCrosshair.enabled = true;
        if (interactCrosshair != null) interactCrosshair.enabled = false;
    }

    void SetupUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0;
            staminaSlider.maxValue = 1;
            staminaSlider.value = 1;
        }
        if (staminaText != null)
            staminaText.text = ((int)maxStamina).ToString();
    }
}