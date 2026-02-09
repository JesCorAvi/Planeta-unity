using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    [Header("Configuración")]
    public GravityAttractor targetPlanet; 
    public float launchForce = 15f;       
    public Color highlightColor = Color.cyan;

    private Color originalColor;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;
    }

    public void ToggleHighlight(bool active)
    {
        if (rend != null)
            rend.material.color = active ? highlightColor : originalColor;
    }

    public void Interact(GameObject player)
    {
        GravityBody playerGravity = player.GetComponent<GravityBody>();
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerGravity != null && targetPlanet != null)
        {
            playerGravity.attractor = targetPlanet;

            if (playerRb != null)
            {
                playerRb.AddForce(player.transform.up * launchForce, ForceMode.Impulse);
            }
            
            Debug.Log("¡Viajando al planeta: " + targetPlanet.name + "!");
        }
    }
}