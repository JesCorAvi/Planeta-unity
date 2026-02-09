using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes")]
    public float moveSpeed = 8f;
    public float mouseSensitivity = 0.2f;
    public float interactionRange = 10f; // Aumentado a 10 metros

    [Header("Referencias")]
    public Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;
    private float cameraPitch = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    
    void OnFire(InputValue value)
    {
        // Si pulsamos el botón, intentamos interactuar
        if (value.isPressed) 
        {
            Debug.Log("BOTÓN DISPARO PULSADO"); // Chivato 1
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        Vector3 targetMove = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y).normalized);
        rb.MovePosition(rb.position + targetMove * moveSpeed * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Dibujar línea roja en la escena (solo visible en el editor) para ver a dónde apuntas
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionRange, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, interactionRange)) 
        {
            Debug.Log("He chocado con: " + hit.collider.name); // Chivato 2: ¿Qué tocamos?

            // Usamos GetComponentInParent por si el script está en el objeto padre
            RockScript rock = hit.collider.GetComponentInParent<RockScript>();
            
            if (rock != null)
            {
                Debug.Log("¡ES UNA ROCA! Rompiendo...");
                rock.Interact();
            }
            else
            {
                Debug.Log("El objeto " + hit.collider.name + " NO tiene el script RockScript.");
            }
        }
        else
        {
            Debug.Log("Disparo al aire (No he tocado nada)");
        }
    }
}