using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq; 

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float mouseSensitivity = 0.2f;

    [Header("Salto Interplanetario")]
    // IMPORTANTE: Sube esto a 50 o 80 en el Inspector para que sea RÁPIDO
    public float jumpForce = 60f; 

    [Header("Interacción")]
    public float interactionRange = 5f;
    public Transform cameraTransform;
    public LayerMask interactableLayer;

    // Variables internas
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;
    private GravityBody gravityBody;
    private float cameraPitch = 0f;
    private RockInteraction currentRock;
    private GravityAttractor[] allPlanets;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        allPlanets = FindObjectsOfType<GravityAttractor>();
    }

    void Update()
    {
        HandleInteractionRaycast();
    }

    // --- LÓGICA DE SALTO ---
    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            GravityAttractor currentPlanet = gravityBody.attractor;
            GravityAttractor targetPlanet = allPlanets.FirstOrDefault(p => p != currentPlanet);

            if (targetPlanet != null)
            {
                // 1. Cambiamos la gravedad al nuevo planeta
                gravityBody.attractor = targetPlanet;

                // 2. Calculamos la dirección directa hacia el centro del otro planeta
                Vector3 direction = (targetPlanet.transform.position - transform.position).normalized;

                // 3. MODO COHETE: Sobrescribimos la velocidad directamente.
                // Esto elimina cualquier frenada o inercia anterior.
                // Al multiplicar por 'jumpForce', sales disparado al instante.
                rb.velocity = direction * jumpForce;
                
                Debug.Log("¡Salto rápido al planeta: " + targetPlanet.name + "!");
            }
        }
    }

    // --- RESTO DEL CÓDIGO (Sin cambios) ---
    void HandleInteractionRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
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

    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    void OnAttack(InputValue value)
    {
        if (value.isPressed && currentRock != null)
        {
            currentRock.Interact();
            currentRock = null;
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
        // Usamos MovePosition para movimiento terrestre, pero el salto usa velocity
        rb.MovePosition(rb.position + targetMove * moveSpeed * Time.fixedDeltaTime);
    }
}