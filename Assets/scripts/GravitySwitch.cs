using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    [Header("Configuración")]
    public GravityAttractor targetPlanet; // Al planeta al que queremos ir
    public float launchForce = 15f;       // Fuerza del impulso
    public Color highlightColor = Color.cyan;

    private Color originalColor;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;
    }

    // Iluminación al mirar (Igual que la roca)
    public void ToggleHighlight(bool active)
    {
        if (rend != null)
            rend.material.color = active ? highlightColor : originalColor;
    }

    // Acción al pulsar
    public void Interact(GameObject player)
    {
        // 1. Obtener el control de gravedad del jugador
        GravityBody playerGravity = player.GetComponent<GravityBody>();
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerGravity != null && targetPlanet != null)
        {
            // 2. Cambiar el atractor (La gravedad se invierte hacia el nuevo planeta)
            playerGravity.attractor = targetPlanet;

            // 3. Dar un empujón hacia arriba para ayudar a salir de la atmósfera actual
            // Usamos transform.up porque en tu sistema la gravedad alinea el 'up' del jugador
            if (playerRb != null)
            {
                playerRb.AddForce(player.transform.up * launchForce, ForceMode.Impulse);
            }
            
            Debug.Log("¡Viajando al planeta: " + targetPlanet.name + "!");
        }
    }
}