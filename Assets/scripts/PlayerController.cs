using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float mouseSensitivity = 0.2f;
    
    [Header("Interacción")]
    public float interactionRange = 5f; 
    public Transform cameraTransform;

    // Variables internas
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;
    private float cameraPitch = 0f;
    
    private RockInteraction currentRock; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleInteractionRaycast();
    }

    void HandleInteractionRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Dibujar línea roja en el editor para ver a dónde apuntas
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionRange, Color.red);

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            RockInteraction rock = hit.collider.GetComponent<RockInteraction>();

            if (rock != null)
            {
                if (currentRock != rock)
                {
                    if (currentRock != null) currentRock.ToggleHighlight(false);
                    currentRock = rock;
                    currentRock.ToggleHighlight(true);
                }
                return; 
            }
        }

        if (currentRock != null)
        {
            currentRock.ToggleHighlight(false);
            currentRock = null;
        }
    }

    // --- SISTEMA DE INPUT ---

    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    // CORRECCIÓN AQUÍ: Antes se llamaba OnFire, ahora es OnAttack
    // porque en tu archivo la acción se llama "Attack"
    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("¡Botón Attack Presionado!"); // Mensaje de confirmación

            if (currentRock != null)
            {
                currentRock.Interact();
                currentRock = null; 
            }
        }
    }

    void LateUpdate()
    {
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void FixedUpdate()
    {
        Vector3 targetMove = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y).normalized);
        rb.MovePosition(rb.position + targetMove * moveSpeed * Time.fixedDeltaTime);
    }
}