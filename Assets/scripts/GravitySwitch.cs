using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GravityAttractor targetPlanet;
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private Color highlightColor = Color.cyan;

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
            playerGravity.Attractor = targetPlanet;

            if (playerRb != null)
            {
                playerRb.AddForce(player.transform.up * launchForce, ForceMode.Impulse);
            }

            Debug.Log("¡Viajando al planeta: " + targetPlanet.name + "!");
        }
    }
}