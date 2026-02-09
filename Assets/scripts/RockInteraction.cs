using UnityEngine;

public class RockInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject lootToSpawn;
    public Color highlightColor = Color.yellow; 

    private Color originalColor;
    private Renderer rend;
    private GravityAttractor myPlanetAttractor; 

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color; 

        GravityBody myGravity = GetComponent<GravityBody>();
        if (myGravity != null)
        {
            myPlanetAttractor = myGravity.attractor;
        }
    }

    public void ToggleHighlight(bool active)
    {
        if (active)
        {
            rend.material.color = highlightColor; 
        }
        else
        {
            rend.material.color = originalColor; 
        }
    }

    public void Interact()
    {
        if (lootToSpawn != null)
        {
            GameObject loot = Instantiate(lootToSpawn, transform.position, transform.rotation);
            
            GravityBody lootGravity = loot.GetComponent<GravityBody>();
            if (lootGravity != null && myPlanetAttractor != null)
            {
                lootGravity.attractor = myPlanetAttractor;
            }
        }
        
        Destroy(gameObject); 
    }
}