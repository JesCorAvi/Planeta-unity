using UnityEngine;

public class RockInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject lootToSpawn;
    public Color highlightColor = Color.yellow; 
    public float spawnHeightOffset = 1.0f; 

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
        else
        {
            var attractors = FindObjectsByType<GravityAttractor>(FindObjectsSortMode.None);
            if (attractors.Length > 0) myPlanetAttractor = attractors[0];
        }
    }

    public void ToggleHighlight(bool active)
    {
        if (rend != null) rend.material.color = active ? highlightColor : originalColor; 
    }

    public void Interact()
    {
        if (lootToSpawn != null)
        {
            Vector3 spawnPos = transform.position + (transform.up * spawnHeightOffset);

            GameObject loot = Instantiate(lootToSpawn, spawnPos, transform.rotation);
            
            GravityBody lootGravity = loot.GetComponent<GravityBody>();
            if (lootGravity != null && myPlanetAttractor != null)
            {
                lootGravity.attractor = myPlanetAttractor;
            }
        }
        
        Destroy(gameObject); 
    }
}