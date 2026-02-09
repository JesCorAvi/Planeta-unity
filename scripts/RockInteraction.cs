using UnityEngine;

public enum BiomeType { Forest, Desert }

public class RockInteraction : MonoBehaviour
{
    public BiomeType currentBiome;
    private bool hasLooted = false;

    // Asigna estos prefabs en el Inspector del Prefab de la Roca, 
    // o cárgalos por código si prefieres.
    public GameObject[] forestCreatures; 
    public GameObject[] desertCreatures;

    public void SetBiome(BiomeType biome)
    {
        currentBiome = biome;
    }

    // Usamos OnMouseDown para prototipo rápido (requiere Collider en la roca)
    private void OnMouseDown()
    {
        Interact();
    }

    public void Interact()
    {
        if (hasLooted) return;

        SpawnLoot();
        hasLooted = true;
        
        // Feedback visual (desaparece o cambia color)
        Destroy(gameObject); 
    }

    void SpawnLoot()
    {
        GameObject creatureToSpawn = null;

        // Elegir criatura según bioma
        if (currentBiome == BiomeType.Forest && forestCreatures.Length > 0)
        {
            creatureToSpawn = forestCreatures[Random.Range(0, forestCreatures.Length)];
        }
        else if (currentBiome == BiomeType.Desert && desertCreatures.Length > 0)
        {
            creatureToSpawn = desertCreatures[Random.Range(0, desertCreatures.Length)];
        }

        if (creatureToSpawn != null)
        {
            GameObject creature = Instantiate(creatureToSpawn, transform.position, transform.rotation);
            
            // Asegurar que la criatura tenga gravedad
            GravityBody gb = creature.GetComponent<GravityBody>();
            if (gb == null) gb = creature.AddComponent<GravityBody>();
            
            // Buscar el atractor más cercano (el planeta)
            gb.attractor = FindObjectOfType<GravityAttractor>();
        }
        else
        {
            Debug.Log("No hay criaturas asignadas para este bioma.");
        }
    }
}