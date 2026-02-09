using UnityEngine;

public class RockInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject lootToSpawn;
    public Color highlightColor = Color.yellow; // Color al mirarla

    // Variables internas
    private Color originalColor;
    private Renderer rend;
    private GravityAttractor myPlanetAttractor; 

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color; // Guardamos el color gris/bioma inicial

        // Buscar gravedad propia para pasarla al loot
        GravityBody myGravity = GetComponent<GravityBody>();
        if (myGravity != null)
        {
            myPlanetAttractor = myGravity.attractor;
        }
    }

    // Se llama cuando el jugador la mira
    public void ToggleHighlight(bool active)
    {
        if (active)
        {
            rend.material.color = highlightColor; // Se ilumina
        }
        else
        {
            rend.material.color = originalColor; // Vuelve a su color normal
        }
    }

    // Se llama cuando el jugador hace CLIC
    public void Interact()
    {
        if (lootToSpawn != null)
        {
            GameObject loot = Instantiate(lootToSpawn, transform.position, transform.rotation);
            
            // Pasamos la gravedad a la criatura para que no salga volando
            GravityBody lootGravity = loot.GetComponent<GravityBody>();
            if (lootGravity != null && myPlanetAttractor != null)
            {
                lootGravity.attractor = myPlanetAttractor;
            }
        }
        
        Destroy(gameObject); // Adios roca
    }
}