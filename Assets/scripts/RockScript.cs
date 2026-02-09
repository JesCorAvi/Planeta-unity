using UnityEngine;

public class RockScript : MonoBehaviour
{
    public GameObject lootToSpawn;

    // Ya no usamos OnMouseDown. Ahora el jugador llama a esta función explícitamente.
    public void Interact()
    {
        if (lootToSpawn != null)
        {
            Instantiate(lootToSpawn, transform.position, transform.rotation);
        }
        
        // Efecto de partículas o sonido iría aquí
        Destroy(gameObject);
    }
}